using UnityEngine;

public class CannonMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f; // Tốc độ di chuyển
    [SerializeField] private Rigidbody rb; // Gán Rigidbody vào Inspector

    private Vector3 moveDirection;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        
        rb.freezeRotation = true; // Ngăn đại bác bị xoay không mong muốn
    }

    void Update()
    {
        // Nhận input từ bàn phím
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Xác định hướng di chuyển
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;
    }

    void FixedUpdate()
    {
        if (moveDirection != Vector3.zero)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
