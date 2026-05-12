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
        if (rb == null) rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
    }

    void Update()
    {
        // Karakter hızı 0 ise (durmuşsa) sağ-sol tuşları çalışmasın
        if (speed > 0)
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("KAZA! Her şey olduğu yerde durdu.");

            // 1. Karakterin hızını sıfırla (Olduğu yerde kalsın)
            speed = 0;

            // 2. Karakterin fiziğini tamamen durdur (Sürüklenmesin)
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true; // Fizik motorunu bu nesne için kapatır (tam donma sağlar)
            }

            // 3. Animasyonu durdur (Eğer karakter hala koşma animasyonu yapıyorsa)
            if (animator != null)
            {
                animator.speed = 0;
            }

            // 4. Sahnedeki TÜM arabaları durdur
            CarMover[] allCars = Object.FindObjectsByType<CarMover>(FindObjectsSortMode.None);
            foreach (CarMover car in allCars)
            {
                car.canMove = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // Karakter durmuşsa fiziksel hareket hesaplama
        if (rb == null || speed <= 0) return;

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