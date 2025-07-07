using UnityEngine;
using UnityEngine.InputSystem;

public class ShadowMerge : MonoBehaviour
{
    private InputAction skill2;
    private bool hiding;
    private float hidingCooldown;
    void Start()
    {
        skill2 = new InputAction(
"Skill2",
InputActionType.Button,
"<Gamepad>/leftTrigger");
        TogglePlayerInput(true);
    }

    public void TogglePlayerInput(bool enable)
    {
        if (enable)
        {
            skill2.Enable();
        }
        if (!enable)
        {
            skill2.Disable();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Shadow" && skill2.IsPressed() && !hiding && hidingCooldown <= 0f)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<NinjaMovement>().enabled = false;
            hiding = true;
            hidingCooldown = 0.4f;
        }
    }

    private void FixedUpdate()
    {
        if (hiding && hidingCooldown <= 0f && skill2.IsPressed())
        {
            hiding = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 3f;
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.GetComponent<NinjaMovement>().enabled = true;
            hidingCooldown = 0.4f;
        }
        hidingCooldown -= Time.fixedDeltaTime;
    }
}
