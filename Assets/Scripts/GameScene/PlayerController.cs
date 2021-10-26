using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float sidewaysSpeed;
    [SerializeField] float maxSpeedBoost;
    [SerializeField] float speedBoostDecay;
    [SerializeField] float jumpForce;
    [SerializeField] float acceleration;
    [SerializeField] float obstacleDamage;
    [SerializeField] float speedBoostCooldownDuration;

    [SerializeField] AudioSource jumpSound;
    [SerializeField] AudioSource hurtSound;
    [SerializeField] AudioSource deadSound;

    private Animator playerAnimator;
    private Rigidbody playerRb;
    private InfiniteDungeon infiniteDungeonScript;
    private ParticleSystem collisionParticle;
    private ParticleSystem dashParticle;
    private Slider healthBar;
    private TextMeshProUGUI scoreText;
    private GameObject gameOverScreen;

    private float speed = 0f;
    private float speedBoost = 0f;
    private float score = 0f;
    private float previousZLocation;
    private bool isOnGround = true;
    private bool isDead = false;
    private bool inDashCooldown = false;


    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();
        infiniteDungeonScript = FindObjectOfType<InfiniteDungeon>();
        collisionParticle = gameObject.transform.Find("CollisionParticle").GetComponent<ParticleSystem>();
        dashParticle = gameObject.transform.Find("WindParticle").GetComponent<ParticleSystem>();
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
        gameOverScreen = GameObject.FindGameObjectWithTag("GameOverScreen");

        previousZLocation = transform.position.z;

        gameOverScreen.SetActive(false);
    }


    void Update()
    {
        if (isDead) return;

        HandleSpeedBoost();
        Move();
        HandleJump();
        HandleRoll();
        UpdateScore();
    }


    void HandleSpeedBoost()
    {
        if (speedBoost > 0)
            speedBoost = Mathf.Max(0, speedBoost - speedBoostDecay * Time.deltaTime);

        if (Input.GetButtonDown("Dash") && !inDashCooldown)
        {
            inDashCooldown = true;
            speedBoost = maxSpeedBoost;
            Invoke("ResetDashCooldown", speedBoostCooldownDuration);

            dashParticle.Play();
        }
    }


    void Move()
    {
        // Accelerate the player
        speed = Mathf.Min(maxSpeed, speed + acceleration * Time.deltaTime);

        Vector3 resultantMovement = Vector3.zero;
        float horizontalInput = Input.GetAxis("Horizontal");

        // Forward movement
        resultantMovement += Vector3.forward * (speed + speedBoost) * Time.deltaTime;
        // Sideways movement
        resultantMovement += Vector3.right * sidewaysSpeed * horizontalInput * Time.deltaTime;

        playerRb.MovePosition( transform.position + resultantMovement);

        // Update the animator
        playerAnimator.SetFloat("speed", speed + speedBoost);
    }


    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            isOnGround = false;
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            playerAnimator.SetTrigger("jump");
            jumpSound.Play();
        }
    }


    void HandleRoll()
    {
        if (Input.GetButtonDown("Roll") && isOnGround)
        {
            playerAnimator.SetTrigger("roll");
        }
    }


    void UpdateScore()
    {
        score += transform.position.z - previousZLocation;
        previousZLocation = transform.position.z;

        scoreText.text = $"Score: { (int)score }";
    }


    void Death()
    {
        playerAnimator.SetTrigger("dead");
        deadSound.Play();
        isDead = true;

        gameOverScreen.SetActive(true);
    }


    void ResetDashCooldown()
    {
        inDashCooldown = false;
    }


    // Used to avoid multiple OnCollisionEnter firing, affecting the health
    // Check if the collided gameobject parent is the same?
    private GameObject previouslyHitObstacle = null;
    private void OnCollisionEnter(Collision collision)
    {
        // Ground
        if (collision.gameObject.CompareTag("Ground"))
            isOnGround = true;
        // Obstacle
        else if (collision.transform.parent.CompareTag("Obstacle"))
        {
            if (previouslyHitObstacle == collision.transform.parent.gameObject) return;

            speed = -7f;
            speedBoost = 0f;
            previouslyHitObstacle = collision.transform.parent.gameObject;
            Destroy(collision.transform.parent.gameObject);

            collisionParticle.Play();

            healthBar.value -= obstacleDamage;
            if (healthBar.value <= 0) Death();
            else
            {
                playerAnimator.SetTrigger("hurt");
                hurtSound.Play();
            }
        }
    }


    // To avoid the player mesh collider (head & body) triggering the dungeon chunk regen 2 times.
    private float previousTriggerLocation = -float.MinValue;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NewChunkTrigger") && other.transform.position.z != previousTriggerLocation)
        {
            previousTriggerLocation = other.transform.position.z;
            infiniteDungeonScript.generateNewChunk();
        }
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
