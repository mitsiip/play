using UnityEngine;
using UnityEngine.UI;

public class PlayerDrone : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float rotationSpeed = 150f;
    public float verticalSpeed = 10f;
    public float maxVelocity = 20f;

    public float health = 100f;
    public float armor = 5f;

    public Transform cargoHold;
    public GameObject currentCargo;

    private Rigidbody rb;

    public Slider healthSlider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 2f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (health <= 0) health = 100f;
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        float liftInput = 0;
        if (Input.GetKey(KeyCode.Space)) liftInput = 1;
        if (Input.GetKey(KeyCode.LeftShift)) liftInput = -1;

        if (turnInput != 0)
        {
            float rotationAmount = turnInput * rotationSpeed * Time.fixedDeltaTime;
            transform.Rotate(0, rotationAmount, 0);
        }

        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        Vector3 liftDirection = Vector3.up * liftInput * verticalSpeed;
        rb.AddForce(moveDirection + liftDirection, ForceMode.Acceleration);

        if (rb.velocity.magnitude > maxVelocity)
            rb.velocity = rb.velocity.normalized * maxVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float damage = collision.relativeVelocity.magnitude * 5f;
        TakeDamage(damage);
    }

    public void TakeDamage(float amount)
    {
        float damageTaken = Mathf.Max(amount - armor, 2);
        health -= damageTaken;
        health = Mathf.Clamp(health, 0, 100);

        healthSlider.value = health;

        if (health <= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}