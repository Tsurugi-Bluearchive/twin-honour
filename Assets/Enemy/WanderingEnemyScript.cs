using Unity.VisualScripting;
using UnityEngine;
public class WanderingEnemyScript : MonoBehaviour
{
    private Rigidbody2D Rigidbody;
    [SerializeField] public float wanderSpeed; // Default speed
    [SerializeField] public float wanderDistance;
    private EnemyVision enemyVision;
    [SerializeField] private GameObject enemyVisionObject;

    private Vector3 _startOfCurrentSegmentPosition;
    [SerializeField] private bool _movingLeft;
    public bool detectedPlayer;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        _startOfCurrentSegmentPosition = transform.position;
        detectedPlayer = false;
        if (enemyVisionObject != null)
        {
            enemyVision = enemyVisionObject.GetComponent<EnemyVision>();
        }
    }

    void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        float currentHorizontalSpeed = _movingLeft ? -wanderSpeed : wanderSpeed;

        if (!detectedPlayer)
        {
            Rigidbody.linearVelocity = new Vector2(currentHorizontalSpeed, Rigidbody.linearVelocityY);
            float distanceTraveled = Mathf.Abs(currentPosition.x - _startOfCurrentSegmentPosition.x);
            // Debug.Log($"Distance Traveled: {distanceTraveled}");
            if (distanceTraveled >= wanderDistance)
            {
                _movingLeft = !_movingLeft;
                _startOfCurrentSegmentPosition = currentPosition;
            }
        }
        if (enemyVision != null)
        {
            enemyVision.left = _movingLeft;
        }
    }
}

