using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    public float moveDuration = 0.3f;
    public int health = 1;
    private Vector3 previousPosition;
    private bool hasAttacked = false;
    private bool hasMoved = false;

    void Start()
    {
        previousPosition = transform.position;
    }

    public void MoveEnemy()
    {
        if (hasMoved) return; // Nếu đã di chuyển, không di chuyển nữa

        previousPosition = transform.position;
        Vector3 moveStep = GetRandomMoveStep();
        Vector3 targetPosition = transform.position + moveStep;

        if (IsInsideBoard(targetPosition) && !IsPositionOccupied(targetPosition))
        {
            hasMoved = true;
            transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                CheckForCombat(targetPosition);
            });
        }
    }

    void CheckForCombat(Vector3 targetPosition)
    {
        if (hasAttacked) return;

        Collider[] colliders = Physics.OverlapSphere(targetPosition, 0.1f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("King"))
            {
                KingController kingController = col.GetComponent<KingController>();
                if (kingController != null)
                {
                    kingController.TakeDamage(1);
                    hasAttacked = true;

                    if (kingController.health > 0)
                    {
                        transform.DOMove(previousPosition, moveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                        {
                            BoardManager.Instance.EndTurn();
                        });
                    }
                    else
                    {
                        Debug.Log("King has died. Game Over!");
                    }
                }
            }
        }
    }

    public void ResetTurnStatus()
    {
        hasAttacked = false;
        hasMoved = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    bool IsInsideBoard(Vector3 position)
    {
        return position.x >= 0 && position.x < 8 && position.z >= 0 && position.z < 8;
    }

    bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy")) // Nếu có Enemy khác ở vị trí đó, không di chuyển vào
            {
                return true;
            }
        }
        return false;
    }

    Vector3 GetRandomMoveStep()
    {
        List<Vector3> possibleMoves = new List<Vector3>
        {
            new Vector3(1, 0, 0), new Vector3(-1, 0, 0), // Trái, phải
            new Vector3(0, 0, 1), new Vector3(0, 0, -1), // Trên, dưới
            new Vector3(1, 0, 1), new Vector3(-1, 0, -1), // Chéo lên phải, chéo xuống trái
            new Vector3(-1, 0, 1), new Vector3(1, 0, -1) // Chéo lên trái, chéo xuống phải
        };

        // Xáo trộn danh sách để đảm bảo ngẫu nhiên
        possibleMoves.Shuffle();

        foreach (Vector3 move in possibleMoves)
        {
            Vector3 targetPosition = transform.position + move;
            if (IsInsideBoard(targetPosition) && !IsPositionOccupied(targetPosition))
            {
                return move; // Chọn nước đi hợp lệ đầu tiên
            }
        }

        return Vector3.zero; // Nếu không có nước đi hợp lệ, đứng yên
    }
}

// Hàm xáo trộn danh sách (Fisher-Yates Shuffle)
public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
