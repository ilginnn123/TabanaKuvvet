using UnityEngine;

public class MyPlayerController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] float speed;
    [SerializeField] private float lateralSmoothSpeed = 10f;

    private float[] xPosition = { -0.0013f, 0.366f, 0.7261f }; 
    private int currentXPositionIndex = 0;
    Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && currentXPositionIndex > 0)
        {
            currentXPositionIndex--;
            UpdateLateralPosition();
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentXPositionIndex < 2)
        {
            currentXPositionIndex++;
            UpdateLateralPosition();
        }
    }

    private void FixedUpdate()
    {
        Vector3 forwardMove = transform.forward * speed * Time.fixedDeltaTime;
        Vector3 currentPosition = rb.position;

       
        Vector3 lateralMove = Vector3.Lerp(currentPosition, new Vector3(targetPosition.x, currentPosition.y, currentPosition.z), Time.fixedDeltaTime * lateralSmoothSpeed);

        rb.MovePosition(lateralMove + forwardMove);
    }

    void UpdateLateralPosition()
    {
        targetPosition = new Vector3(xPosition[currentXPositionIndex], transform.position.y, transform.position.z);
    }
}