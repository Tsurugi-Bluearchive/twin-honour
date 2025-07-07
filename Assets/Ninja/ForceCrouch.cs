using UnityEngine;

public class ForceCrouch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created\
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ceiling")
        {
            gameObject.GetComponentInParent<NinjaMovement>().forceCrouch = true;
        }
        else
        {
            gameObject.GetComponentInParent<NinjaMovement>().forceCrouch = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        gameObject.GetComponentInParent<NinjaMovement>().forceCrouch = false;
    }

}
