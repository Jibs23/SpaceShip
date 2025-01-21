using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public HealthScript Health;
    public Rigidbody2D myRigidbody;
    public GameObject bulletPrefab;
    public LogicScript Logic;
    public Animator Animator;
    public HealthUiScript HealthUI;
    private SpriteRenderer spriteRenderer;
    public ParticleSystem ThrustParticles;
    public float moveSpeed = 5f;
    public float rotationSpeed = 1250f;
    public float bulletCooldown = 0.5f;
    public bool IsThrusting;
    public bool IsOffScreen;
    public bool IsDying;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        HealthUI = GameObject.FindGameObjectWithTag("HealthUI").GetComponent<HealthUiScript>();
        Health = GetComponent<HealthScript>();
        Animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        IsThrusting = false; // Reset IsThrusting at the start of each frame
        if (transform.position.x > Logic.RightSideOfScreenInWorld || transform.position.x < Logic.LeftSideOfScreenInWorld || transform.position.y > Logic.TopOfScreenInWorld || transform.position.y < Logic.BottomOfScreenInWorld)
        {
            IsOffScreen = true;
        }
        else
        {
            IsOffScreen = false;
        }



        // Handle key inputs
        foreach (KeyCode key in new KeyCode[] { KeyCode.W, KeyCode.D, KeyCode.A, KeyCode.Space }) // Loop through the keys we want to check for input
        {
            if (Input.GetKey(key)) // Check if the key is being held down
            {
                switch (key) // Check which key is being held down
                {
                    case KeyCode.W: // move forward
                        myRigidbody.AddForce(transform.up * moveSpeed);
                        IsThrusting = true; 
                        break;
                    case KeyCode.D: // turn right
                        myRigidbody.AddTorque(-rotationSpeed * Time.deltaTime);
                        break;
                    case KeyCode.A: // turn left
                        myRigidbody.AddTorque(rotationSpeed * Time.deltaTime);
                        break;
                    case KeyCode.Space: // Shoot
                        Shoot();
                        break;
                }
            }
        }
        // PARTICLES
        if (IsThrusting)
        {
            ThrustParticles.Play();
        }
        else
        {
            ThrustParticles.Stop();
        }

        // HEALTH UI
        if (Health != null && Health.CurrentHealth != Health.MaxHealth)
        {
            UpdateHealthEndicator();
        }

        //* WHEN YOU DIE
        if (Health != null && Health.IsDead)
        {
            Die();
        }
        Animator.SetBool("IsThrusting", IsThrusting); // Set the IsThrusting parameter in the Animator to the value of IsThrusting

    }
    void UpdateHealthEndicator()
    {
        if (Health == null || HealthUI == null)
        {
            Debug.LogError("<color=red>Health or HealthUI is not assigned.</color>");
            return;
        }

        if (Health.CurrentHealth ==  1)
        {
            HealthUI.LoseH3();
            IsDying = true;
        }
        else if (Health.CurrentHealth == 2)
        {
            HealthUI.LoseH2();
        }
        else if (Health.CurrentHealth == 3)
        {
            HealthUI.LoseH1();
        }
    }


    private float lastShotTime = 0f;
    void Shoot()
    {
        if (Time.time > lastShotTime + bulletCooldown) // Check if enough time has passed since the last shot by comparing the current time to the last shot time plus the cooldown
        {
            // Create a new bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            
            // Orient the bullet in the same direction as the rigid body
            bullet.transform.rotation = myRigidbody.transform.rotation;

            // Add a number to the bullet's y transform relative to Player object's rotation
            bullet.transform.position += transform.up;

            // Update the last shot time
            lastShotTime = Time.time;
        }
    }
    public void Die()
    {
        Logic.isGameOver = true;
        Destroy(gameObject);
    }

}
