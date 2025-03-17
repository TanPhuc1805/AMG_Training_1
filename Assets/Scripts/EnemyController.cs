using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    public float moveDuration = 0.3f;
    public int health = 1;
    private Vector3 previousPosition;
    private bool hasAttacked = false;
    private bool hasMoved = false;

    public EnemyType enemyType; // Sử dụng enum mới

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
        return Physics.OverlapSphere(position, 0.1f).Any(col => col.CompareTag("Enemy"));
    }

    Vector3 GetRandomMoveStep()
    {
        List<Vector3> possibleMoves = new List<Vector3>();

        switch (enemyType)
        {
            case EnemyType.Rook:
                possibleMoves.AddRange(GetLineMoves(new Vector3(1, 0, 0)));  
                possibleMoves.AddRange(GetLineMoves(new Vector3(-1, 0, 0))); 
                possibleMoves.AddRange(GetLineMoves(new Vector3(0, 0, 1)));  
                possibleMoves.AddRange(GetLineMoves(new Vector3(0, 0, -1))); 
                break;

            case EnemyType.Bishop:
                possibleMoves.AddRange(GetLineMoves(new Vector3(1, 0, 1)));  
                possibleMoves.AddRange(GetLineMoves(new Vector3(-1, 0, -1))); 
                possibleMoves.AddRange(GetLineMoves(new Vector3(-1, 0, 1)));  
                possibleMoves.AddRange(GetLineMoves(new Vector3(1, 0, -1)));  
                break;

            case EnemyType.Knight:
                possibleMoves = new List<Vector3>
                {
                    new Vector3(2, 0, 1), new Vector3(2, 0, -1),
                    new Vector3(-2, 0, 1), new Vector3(-2, 0, -1),
                    new Vector3(1, 0, 2), new Vector3(1, 0, -2),
                    new Vector3(-1, 0, 2), new Vector3(-1, 0, -2)
                };
                break;

            case EnemyType.Queen:
                possibleMoves.AddRange(GetLineMoves(new Vector3(1, 0, 0)));  
                possibleMoves.AddRange(GetLineMoves(new Vector3(-1, 0, 0))); 
                possibleMoves.AddRange(GetLineMoves(new Vector3(0, 0, 1)));  
                possibleMoves.AddRange(GetLineMoves(new Vector3(0, 0, -1))); 
                possibleMoves.AddRange(GetLineMoves(new Vector3(1, 0, 1)));  
                possibleMoves.AddRange(GetLineMoves(new Vector3(-1, 0, -1))); 
                possibleMoves.AddRange(GetLineMoves(new Vector3(-1, 0, 1)));  
                possibleMoves.AddRange(GetLineMoves(new Vector3(1, 0, -1)));  
                break;

            case EnemyType.Pawn:
                possibleMoves.Add(new Vector3(0, 0, 1)); 
                possibleMoves.Add(new Vector3(1, 0, 1)); 
                possibleMoves.Add(new Vector3(-1, 0, 1)); 
                break;

            case EnemyType.King:
                possibleMoves = new List<Vector3>
                {
                    new Vector3(1, 0, 0), new Vector3(-1, 0, 0),
                    new Vector3(0, 0, 1), new Vector3(0, 0, -1),
                    new Vector3(1, 0, 1), new Vector3(-1, 0, -1),
                    new Vector3(-1, 0, 1), new Vector3(1, 0, -1)
                };
                break;
        }

        possibleMoves.Shuffle();

        foreach (Vector3 move in possibleMoves)
        {
            Vector3 targetPosition = transform.position + move;
            if (IsInsideBoard(targetPosition) && !IsPositionOccupied(targetPosition))
            {
                return move;
            }
        }

        return Vector3.zero;
    }

    List<Vector3> GetLineMoves(Vector3 direction)
    {
        List<Vector3> moves = new List<Vector3>();
        Vector3 position = transform.position + direction;

        while (IsInsideBoard(position) && !IsPositionOccupied(position))
        {
            moves.Add(position - transform.position);
            position += direction;
        }
        return moves;
    }
}

// Fisher-Yates Shuffle để xáo trộn danh sách
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
