using UnityEngine;

public class CarMover : MonoBehaviour
{
    [SerializeField] private float carSpeed = 5f;
    public bool canMove = true; // Arabanın hareket izni

    void Update()
    {
        if (canMove)
        {
            transform.Translate(0, 0, carSpeed * Time.deltaTime);
        }
    }
}