using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBallCoroutine : MonoBehaviour
{
    public float bouncingStrength = 0.8f; 
    public float force = 5f; 
    public float angle = 45f; 
    public float gravity = 9.8f; 
    public float floorY = 0f; 
    public float stopThreshold = 0.001f; 
    private Vector2 velocity;
    private bool isMoving = true;

    void Start()
    {
        float rad = angle * Mathf.Deg2Rad;
        velocity = new Vector2(Mathf.Cos(rad) * force, Mathf.Sin(rad) * force);
        StartCoroutine(Bounce());
    }

    IEnumerator Bounce()
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

            yield return null;
        }
    }
}
