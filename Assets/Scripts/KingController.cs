using UnityEngine;
using DG.Tweening;

public class KingController : MonoBehaviour
{
    public float moveDuration = 0.3f;
    public int health = 10;
    private bool isMoving = false;
    private Vector3 previousPosition; // Lưu vị trí cũ nếu King bị đẩy lùi

    void Update()
    {

        if (!isMoving)
        {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W)) move = new Vector3(0, 0, 1);
        if (Input.GetKeyDown(KeyCode.S)) move = new Vector3(0, 0, -1);
        if (Input.GetKeyDown(KeyCode.A)) move = new Vector3(-1, 0, 0);
        if (Input.GetKeyDown(KeyCode.D)) move = new Vector3(1, 0, 0);
        if (Input.GetKeyDown(KeyCode.Q)) move = new Vector3(-1, 0, 1);
        if (Input.GetKeyDown(KeyCode.E)) move = new Vector3(1, 0, 1);
        if (Input.GetKeyDown(KeyCode.Z)) move = new Vector3(-1, 0, -1);
        if (Input.GetKeyDown(KeyCode.C)) move = new Vector3(1, 0, -1);

        if (move != Vector3.zero)
        {
            AttemptMove(move);
            Debug.Log("Attempt");
        }
    }

    void AttemptMove(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + direction;
        if (IsInsideBoard(targetPosition))
        {
            Debug.Log("Move");
            previousPosition = transform.position; // Lưu vị trí cũ trước khi di chuyển
            isMoving = true;
            transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                CheckForCombat(targetPosition);
            });
        }
    }

    void CheckForCombat(Vector3 targetPosition)
    {
        Collider[] colliders = Physics.OverlapSphere(targetPosition, 0.1f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                EnemyController enemy = col.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1);

                    if (enemy.health > 0)
                    {
                        // Nếu Enemy chưa chết, King quay lại vị trí cũ
                        transform.DOMove(previousPosition, moveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                        {
                            isMoving = false;
                            BoardManager.Instance.EndTurn();
                        });
                        return;
                    }
                }
            }
        }

        // Nếu không có enemy hoặc enemy đã chết, kết thúc lượt
        isMoving = false;
        BoardManager.Instance.EndTurn();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Game Over! King has died.");
            Destroy(gameObject);
        }
    }

    bool IsInsideBoard(Vector3 position)
    {
        return position.x >= 0 && position.x < 8 && position.z >= 0 && position.z < 8;
    }

    public void EnableMovement()
    {
        isMoving = false;
    }
}
