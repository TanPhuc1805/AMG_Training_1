using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BouncingBallUpdate : MonoBehaviour
{
    public float bouncingStrength = 0.8f; // Hệ số nảy
    public float force = 5f; // Lực ném ban đầu
    public float angle = 45f; // Góc ném ban đầu
    public float gravity = 9.8f; // Trọng lực
    public float floorY = 0f; // Vị trí sàn
    public float stopThreshold = 0.001f; // Ngưỡng dừng bóng

    private Vector2 velocity;
    private bool isMoving = true; 

    void Start()
    {
        float rad = angle * Mathf.Deg2Rad;
        velocity = new Vector2(Mathf.Cos(rad) * force, Mathf.Sin(rad) * force);
        StartBouncing().Forget(); // Chạy UniTask mà không cần `await`
    }
    
    async UniTaskVoid StartBouncing()
    {
        while (isMoving)
        {
            velocity.y -= gravity * Time.deltaTime;
            transform.position += (Vector3)(velocity * Time.deltaTime);

            if (transform.position.y <= floorY)
            {
                velocity.y *= -bouncingStrength;
                velocity.x *= bouncingStrength;
                transform.position = new Vector3(transform.position.x, floorY, transform.position.z);

                if (Mathf.Abs(velocity.y) < stopThreshold)
                {
                    isMoving = false;
                    velocity = Vector2.zero;
                }
            }

            await UniTask.Yield(PlayerLoopTiming.Update); // Chờ đến frame tiếp theo
        }
    }
}
