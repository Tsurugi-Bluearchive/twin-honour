using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class NinjaMovement : MonoBehaviour
{
    private InputAction move;
    private InputAction look;
    private InputAction jump;
    private float moveAmplitude;
    public float baseMoveAmplitude;
    public float jumpAmplitude;
    private float jumpControl;
    private float jumpCooldown;
    private float climbAmplitude;
    private bool climbing;
    private bool hanging;
    private bool crouching;
    public bool forceCrouch;
    private byte jumpCount;
    private bool dashable;
    private Vector3 previousGlobalPosition;
    public Vector3 momentum;
    [SerializeField] private GameObject forceCrouchObject;
    private ForceCrouch ForceCrouchScript;
    private Rigidbody2D Rigidbody2D;
    [SerializeField] private GameObject playerCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jumpCooldown = 0;
        climbAmplitude = 20;
        move = new InputAction(
"Move",
InputActionType.Value,
"<Gamepad>/leftStick");

        look = new InputAction(
"Look",
InputActionType.Value,
"<Gamepad>/rightStick");

        jump = new InputAction(
            "Jump",
            InputActionType.Button,
            "<Gamepad>/buttonSouth");

        baseMoveAmplitude = 20f;
        TogglePlayerInput(true);
        Rigidbody2D = GetComponent<Rigidbody2D>();
        jumpAmplitude = 20f;
        ForceCrouchScript = forceCrouchObject.GetComponent<ForceCrouch>();
        jumpCount = 2;
    }

    public void TogglePlayerInput(bool enable)
    {
        if (enable)
        {
            move.Enable();
            look.Enable();
            jump.Enable();
        }
        if (!enable)
        {
            move.Disable();
            look.Disable();
            jump.Disable();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        momentum = Rigidbody2D.linearVelocity;
        forceCrouchObject.SetActive(crouching);
        
        
        //NinjaMovement.cs Jump Logic
        if (jump.IsPressed() && jumpCooldown <= 0 && !hanging && !climbing && jumpCount >= 1 && !dashable)
        {
            jumpCount--;
            jumpControl = jumpAmplitude;
            jumpCooldown = 0.4f;
        }
        else
        {
            jumpControl = 0f;
            jumpCooldown -= Time.fixedDeltaTime;
        }

        if (dashable && jump.IsPressed() && jumpCooldown <= 0 && !hanging && !climbing && jumpCount >= 1)
        {
            moveAmplitude = baseMoveAmplitude * 3;
            jumpCount--;
        }
        else
        {
            moveAmplitude = baseMoveAmplitude;
        }
            //NinjaMovement.cs Movement Logic
        if (move.ReadValue<Vector2>().y < -0.2f && !crouching && !climbing && !hanging)
        {
            transform.localScale = new Vector3(1, Mathf.Lerp(2.5f, 5, move.ReadValue<Vector2>().y), 1);
            transform.position = new Vector3(transform.position.x, transform.position.y - 1.25f, transform.position.z);
            crouching = true;
            moveAmplitude = baseMoveAmplitude / 2;
        }
        else if (!climbing && !hanging)
        {
            if (Rigidbody2D.linearVelocity.x < 20f && Rigidbody2D.linearVelocity.x > -20f)
            {
                Rigidbody2D.linearVelocity = new Vector2(move.ReadValue<Vector2>().x * moveAmplitude, momentum.y + jumpControl);
            }
            else if (Rigidbody2D.linearVelocity.x > 20f && move.ReadValue<Vector2>().x < 0f)
            {
                Rigidbody2D.linearVelocity = new Vector2(momentum.x + move.ReadValue<Vector2>().x * moveAmplitude, momentum.y + jumpControl);
            }
            else if (Rigidbody2D.linearVelocity.x < -20f && move.ReadValue<Vector2>().x > 0f)
            {
                Rigidbody2D.linearVelocity = new Vector2(momentum.x + move.ReadValue<Vector2>().x * moveAmplitude, momentum.y + jumpControl);
            }
            else
            {
                Rigidbody2D.linearVelocity = new Vector2(momentum.x, momentum.y + jumpControl);
            }
        }

        if (move.ReadValue<Vector2>().y >= -0.2f && !forceCrouch)
        {
            transform.localScale = new Vector3(1, 5, 1);
            crouching = false;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        dashable = false;
        if (collision.gameObject.tag == "ClimbableWall")
        {
            if (transform.position.x < collision.transform.position.x)
            {
                climbing = true;
                if (move.ReadValue<Vector2>().x < 0f && jump.IsPressed())
                {
                    Rigidbody2D.linearVelocity = new Vector2(move.ReadValue<Vector2>().x * moveAmplitude, 0f);
                }
                else if (move.ReadValue<Vector2>().x < 0f)
                {
                    playerCamera.gameObject.transform.position = new Vector3(move.ReadValue<Vector2>().x * 3 + transform.position.x, move.ReadValue<Vector2>().y * 3 + transform.position.y, -10);
                    Stick(previousGlobalPosition);
                }
                else
                {
                    playerCamera.gameObject.transform.localPosition = new Vector3(0, 0, -20);
                    Rigidbody2D.linearVelocity = new Vector2(0f, move.ReadValue<Vector2>().y * climbAmplitude);
                    previousGlobalPosition = transform.position;
                }
            }
            else
            {
                climbing = true;
                if (move.ReadValue<Vector2>().x > 0f && jump.IsPressed())
                {
                    Rigidbody2D.linearVelocity = new Vector2(move.ReadValue<Vector2>().x * moveAmplitude, 0f);
                }
                else if (move.ReadValue<Vector2>().x > 0f)
                {
                    playerCamera.gameObject.transform.position = new Vector3(move.ReadValue<Vector2>().x * 3 + transform.position.x, move.ReadValue<Vector2>().y * 3 + transform.position.y, -10);
                    Stick(previousGlobalPosition);
                }
                else
                {
                    playerCamera.gameObject.transform.localPosition = new Vector3(0, 0, -20);
                    Rigidbody2D.linearVelocity = new Vector2(0f, move.ReadValue<Vector2>().y * climbAmplitude);
                    previousGlobalPosition = transform.position;
                }
            }
        }
        else if (collision.gameObject.tag == "ClimbableCeiling")
        {
            hanging = true;
            if (move.ReadValue<Vector2>().y >= -0.2f)
            {
                Rigidbody2D.linearVelocity = new Vector2(move.ReadValue<Vector2>().x * moveAmplitude, 5f);
            }
            else if (move.ReadValue<Vector2>().y < -0.2f && jump.IsPressed())
            {
                hanging = false;
            }
        }
        else if (collision.gameObject.tag == "Ceiling")
        {
            jumpCount = 1;
        }
    }

    private void Stick(Vector3 previousPosition)
    {
        transform.position = previousPosition;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        dashable = true;
        climbing = false;
        hanging = false;
    }
}
