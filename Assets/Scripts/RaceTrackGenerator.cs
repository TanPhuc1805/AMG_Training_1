using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RaceTrackGenerator : MonoBehaviour
{
    public float size_x = 50f;      // Tổng chiều dài đường đua
    public float size_z = 10f;      // Chiều rộng đường đua
    public float trackWidth = 2f;   // Độ rộng đường đua
    public int segments = 32;       // Độ mịn của góc bo tròn

    private Mesh mesh;

    void Start()
    {
        GenerateTrack();
    }

    void GenerateTrack()
    {
        mesh = new Mesh();
        mesh.name = "Race Track";
        GetComponent<MeshFilter>().mesh = mesh;

        bool isHorizontal = size_x >= size_z;

        float outerRadius = (isHorizontal ? size_z : size_x) / 2f;
        float innerRadius = ((isHorizontal ? size_z : size_x) - trackWidth) / 2f;
        float straightLength = Mathf.Abs(size_x - size_z);

        Vector3 rightCenter = isHorizontal ? new Vector3(straightLength / 2f, 0, 0) : new Vector3(0, 0, straightLength / 2f);
        Vector3 leftCenter = isHorizontal ? new Vector3(-straightLength / 2f, 0, 0) : new Vector3(0, 0, -straightLength / 2f);

        int vertCount = segments * 4;
        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[segments * 12];

        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.PI * i / (segments - 1);
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            if (isHorizontal)
            {
                vertices[i] = rightCenter + new Vector3(sin * outerRadius, 0, cos * outerRadius);
                vertices[i + segments] = rightCenter + new Vector3(sin * innerRadius, 0, cos * innerRadius);
                vertices[i + segments * 2] = leftCenter + new Vector3(-sin * outerRadius, 0, -cos * outerRadius);
                vertices[i + segments * 3] = leftCenter + new Vector3(-sin * innerRadius, 0, -cos * innerRadius);
            }
            else
            {
                vertices[i] = rightCenter + new Vector3(cos * outerRadius, 0, sin * outerRadius);
                vertices[i + segments] = rightCenter + new Vector3(cos * innerRadius, 0, sin * innerRadius);
                vertices[i + segments * 2] = leftCenter + new Vector3(-cos * outerRadius, 0, -sin * outerRadius);
                vertices[i + segments * 3] = leftCenter + new Vector3(-cos * innerRadius, 0, -sin * innerRadius);
            }
        }

        int t = 0;
        for (int i = 0; i < segments - 1; i++)
        {
            if (isHorizontal)
            {
                triangles[t++] = i;
                triangles[t++] = i + 1;
                triangles[t++] = i + segments;

                triangles[t++] = i + 1;
                triangles[t++] = i + segments + 1;
                triangles[t++] = i + segments;

                triangles[t++] = i + segments * 2;
                triangles[t++] = i + segments * 2 + 1;
                triangles[t++] = i + segments * 3;

                triangles[t++] = i + segments * 2 + 1;
                triangles[t++] = i + segments * 3 + 1;
                triangles[t++] = i + segments * 3;
            }
            else
            {
                triangles[t++] = i;
                triangles[t++] = i + segments;
                triangles[t++] = i + 1;

                triangles[t++] = i + 1;
                triangles[t++] = i + segments;
                triangles[t++] = i + segments + 1;

                triangles[t++] = i + segments * 2;
                triangles[t++] = i + segments * 3;
                triangles[t++] = i + segments * 2 + 1;

                triangles[t++] = i + segments * 2 + 1;
                triangles[t++] = i + segments * 3;
                triangles[t++] = i + segments * 3 + 1;
            }
        }
        // Nối hai đường thẳng giữa 2 bán nguyệt
        if (isHorizontal)
        {
            triangles[t++] = segments - 1;
            triangles[t++] = segments * 2;
            triangles[t++] = segments + segments - 1;

            triangles[t++] = segments + segments - 1;
            triangles[t++] = segments * 2;
            triangles[t++] = segments * 3;

            triangles[t++] = 0;
            triangles[t++] = segments;
            triangles[t++] = segments * 2 + segments - 1;

            triangles[t++] = segments;
            triangles[t++] = segments * 3 + segments - 1;
            triangles[t++] = segments * 2 + segments - 1;
        }
        else
        {
            triangles[t++] = segments - 1;
            triangles[t++] = segments + segments - 1;
            triangles[t++] = segments * 2;

            triangles[t++] = segments + segments - 1;
            triangles[t++] = segments * 3;
            triangles[t++] = segments * 2;

            triangles[t++] = 0;
            triangles[t++] = segments * 2 + segments - 1;
            triangles[t++] = segments;

            triangles[t++] = segments;
            triangles[t++] = segments * 2 + segments - 1;
            triangles[t++] = segments * 3 + segments - 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        Vector2[] uvs = new Vector2[vertCount];

        for (int i = 0; i < segments; i++)
        {
            float u = (float)i / (segments - 1); // Tạo tọa độ U (horizontally mapped)
            uvs[i] = new Vector2(u, 1); 
            uvs[i + segments] = new Vector2(u, 0);
            uvs[i + segments * 2] = new Vector2(u, 1);
            uvs[i + segments * 3] = new Vector2(u, 0);
        }

        // Gán UV vào mesh
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(size_x, 0, size_z));
    }
}