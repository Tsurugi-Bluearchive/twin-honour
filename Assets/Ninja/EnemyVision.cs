using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Vector3 offeset;
    [SerializeField] public bool left;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition = left ? new Vector3(-offeset.x, offeset.y, offeset.z) : new Vector3(offeset.x, offeset.y, offeset.z);
    }
}
