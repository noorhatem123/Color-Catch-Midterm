using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component attached to the player
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Reset velocity to avoid unwanted movement
        rb.velocity = Vector3.zero;

        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Create a movement vector
        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized * speed;

        // Apply movement to the Rigidbody
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }
}
