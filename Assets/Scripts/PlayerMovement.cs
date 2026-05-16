using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    // --- State Machine ---
    public enum PlayerState
    {
        Running,
        Dying
    }
    public PlayerState currentState = PlayerState.Running;

    // --- Movement Settings ---
    [Header("Movement Properties")]
    public float forwardSpeed = 10f;
    public float laneSwitchSpeed = 10f;

    private float[] xPosition = { 0f, 0.375f, 0.75f };
    private int currentLane = 1;

    // --- Car Spawner Tracking ---
    [Header("Spawner Tracking")]
    [SerializeField] private Transform carSpawner; // Drop your Car Spawner object here in the Inspector
    private float initialSpawnerZOffset; // Stores the starting distance between player and spawner
    public GameObject gameEndPanel;
    public GameManager gameManager;

    // --- Components ---
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        animator.SetBool("isAlive", true);

        gameEndPanel.SetActive(false);


        // Snap instantly to the starting lane position
        Vector3 startPos = rb.position;
        startPos.x = xPosition[currentLane];
        rb.position = startPos;

        // Calculate and save the initial distance on the Z-axis between the player and the spawner
        if (carSpawner != null)
        {
            initialSpawnerZOffset = carSpawner.position.z - rb.position.z;
        }
        else
        {
            Debug.LogWarning("Car Spawner is not assigned in the PlayerMovement script!");
        }
    }

    void Update()
    {
        if (currentState == PlayerState.Running)
        {
            HandleInput();
        }
    }

    void FixedUpdate()
    {
        if (currentState == PlayerState.Running)
        {
            MoveForward();
            SnapToLane();
            UpdateSpawnerPosition(); // Keeps the spawner moving forward with us
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentLane--;
            if (currentLane < 0) currentLane = 0;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            currentLane++;
            if (currentLane > 2) currentLane = 2;
        }
    }

    private void MoveForward()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, forwardSpeed);
    }

    private void SnapToLane()
    {
        float targetX = xPosition[currentLane];
        Vector3 targetPosition = new Vector3(targetX, rb.position.y, rb.position.z);
        Vector3 newPosition = Vector3.Lerp(rb.position, targetPosition, Time.fixedDeltaTime * laneSwitchSpeed);
        rb.MovePosition(newPosition);
    }

    private void UpdateSpawnerPosition()
    {
        // Safety check to ensure a spawner was actually assigned
        if (carSpawner != null)
        {
            // Maintain its original X and Y coordinates, but update Z relative to the player's current Z
            Vector3 targetSpawnerPos = new Vector3(
                carSpawner.position.x,
                carSpawner.position.y,
                rb.position.z + initialSpawnerZOffset
            );

            carSpawner.position = targetSpawnerPos;
        }
    }

    // --- Collision & Death Logic ---
    private void OnTriggerEnter(Collider other)
    {
        if (currentState == PlayerState.Running && other.CompareTag("Enemy"))
        {
            Die();
        }
    }

    private void Die()
    {
        currentState = PlayerState.Dying;
        rb.linearVelocity = Vector3.zero;
        animator.SetBool("isAlive", false);
        Debug.Log("Player hit an obstacle! State changed to Dying.");
        //OLUNCE UI GAME END GELSIN
        gameEndPanel.SetActive(true);
        gameManager.SetFinalScore();

    }

    public void RestartGame()
    {
        int gameScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(gameScene);
    }
}