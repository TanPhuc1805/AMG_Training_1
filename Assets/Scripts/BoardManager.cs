using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject kingPrefab;

    // Prefab riêng cho từng loại quân địch
    public GameObject pawnPrefab;
    public GameObject rookPrefab;
    public GameObject bishopPrefab;
    public GameObject knightPrefab;
    public GameObject queenPrefab;
    public GameObject enemyKingPrefab;

    public int boardSize = 8;
    public static BoardManager Instance;

    private List<Vector3> availablePositions = new List<Vector3>();
    private GameObject king;
    private List<GameObject> enemies = new List<GameObject>();

    private bool isKingTurn = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        GenerateBoard();
        PlaceUnits();
    }

    void GenerateBoard()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                Vector3 position = new Vector3(x, 0, z);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);

                tile.GetComponent<Renderer>().material.color = (x + z) % 2 == 0 ? Color.black : Color.white;
                availablePositions.Add(position);
            }
        }
    }

    void PlaceUnits()
{
    // Đặt King
    Vector3 kingPosition = new Vector3(4, 0.5f, 0);
    GameObject myKing = Instantiate(kingPrefab, kingPosition, Quaternion.identity);
    myKing.tag = "King";
    myKing.AddComponent<KingController>();

    // Thêm Collider và Rigidbody cho King
    if (myKing.GetComponent<BoxCollider>() == null)
    {
        BoxCollider collider = myKing.AddComponent<BoxCollider>();
        collider.isTrigger = true;
    }
    if (myKing.GetComponent<Rigidbody>() == null)
    {
        Rigidbody rb = myKing.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    availablePositions.Remove(kingPosition);
    king = myKing;

    // Đặt số lượng quân địch ngẫu nhiên (từ 5 đến 10)
    int enemyCount = Random.Range(5, 11);

    for (int i = 0; i < enemyCount; i++)
    {
        Vector3 enemyPosition = GetRandomAvailablePosition();
        EnemyType enemyType = GetRandomEnemyType();
        GameObject enemyPrefab = GetPrefabByType(enemyType);

        if (enemyPrefab == null)
        {
            Debug.LogError("Missing Prefab for: " + enemyType);
            continue;
        }

        GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        enemy.tag = "Enemy";

        // Đặt màu đen cho quân địch
        Renderer enemyRenderer = enemy.GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.black;
        }
        else
        {
            Debug.LogError("Enemy prefab missing Renderer component: " + enemyType);
        }

        // Gán loại enemy
        EnemyController enemyController = enemy.AddComponent<EnemyController>();
        enemyController.enemyType = enemyType;

        // Thêm Collider và Rigidbody cho Enemy
        if (enemy.GetComponent<BoxCollider>() == null)
        {
            BoxCollider collider = enemy.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }
        if (enemy.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = enemy.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        enemies.Add(enemy);
    }
}


    Vector3 GetRandomAvailablePosition()
    {
        int index = Random.Range(0, availablePositions.Count);
        Vector3 position = availablePositions[index];
        availablePositions.RemoveAt(index);
        return new Vector3(position.x, 0.5f, position.z);
    }

    EnemyType GetRandomEnemyType()
    {
        // Lấy một loại quân ngẫu nhiên từ danh sách Enum
        EnemyType[] enemyTypes = (EnemyType[])System.Enum.GetValues(typeof(EnemyType));
        return enemyTypes[Random.Range(0, enemyTypes.Length)];
    }

    GameObject GetPrefabByType(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Pawn: return pawnPrefab;
            case EnemyType.Rook: return rookPrefab;
            case EnemyType.Bishop: return bishopPrefab;
            case EnemyType.Knight: return knightPrefab;
            case EnemyType.Queen: return queenPrefab;
            case EnemyType.King: return enemyKingPrefab;
            default: return null;
        }
    }

    public void EndTurn()
    {
        if (isKingTurn)
        {
            isKingTurn = false;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            isKingTurn = true;
            king.GetComponent<KingController>().EnableMovement();
        }
    }

    IEnumerator EnemyTurn()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.GetComponent<EnemyController>().ResetTurnStatus();
            }
        }

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.GetComponent<EnemyController>().MoveEnemy();
                yield return new WaitForSeconds(0.3f);
            }
        }

        EndTurn();
    }
}
