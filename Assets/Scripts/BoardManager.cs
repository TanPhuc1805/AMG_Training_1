using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject kingPrefab;
    public GameObject enemyPrefab;
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
    king=myKing;

    // Đặt Enemy
    for (int i = 0; i < 8; i++)
    {
        Vector3 enemyPosition = GetRandomAvailablePosition();
        GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        enemy.tag = "Enemy";
        enemy.GetComponent<Renderer>().material.color = Color.black;
        enemy.AddComponent<EnemyController>();

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
