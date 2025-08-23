using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public Material floorMaterial;
    public int width = 150;       // 바닥 가로 크기
    public int depth = 100;       // 바닥 세로 크기

    Mesh mesh;
    MeshFilter mf;
    MeshRenderer mr;
    private int tilesPerRow = 4; // 아틀라스 한 줄 타일 수
    private float tileSize;

    private void Awake()
    {
        mf = gameObject.AddComponent<MeshFilter>();
        mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = floorMaterial;

        mesh = new Mesh();
        tileSize = (1f / tilesPerRow);
    }

    void Start()
    {
        //for (int x = 0; x < width; x++)
        //{
        //    for (int z = 0; z < depth; z++)
        //    {
        //        Vector3 pos = new Vector3(x, 0, z); // Y축 0 위치에 배치 (바닥)
        //        Instantiate(cubePrefab, pos, Quaternion.identity);
        //    }
        //}

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        int vertIndex = 0;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < depth; j++) {
                // 윗면 4개 꼭짓점 (y=0)
                vertices.Add(new Vector3(i, 0, j));
                vertices.Add(new Vector3(i + 1, 0, j));
                vertices.Add(new Vector3(i, 0, j + 1));
                vertices.Add(new Vector3(i + 1, 0, j + 1));

                // 삼각형 두 개로 윗면 만들기
                triangles.Add(vertIndex);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 1);

                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 3);

                // UV 매핑 (간단히 0~1 범위)
                //uvs.Add(new Vector2(0, 0));
                //uvs.Add(new Vector2(1, 0));
                //uvs.Add(new Vector2(0, 1));
                //uvs.Add(new Vector2(1, 1));

                // 타일마다 UV 오프셋 계산 (예: 랜덤으로 선택)
                int tileX = Random.Range(0, tilesPerRow);
                int tileY = Random.Range(0, tilesPerRow);

                float uvBaseX = tileX * tileSize;
                float uvBaseY = tileY * tileSize;

                uvs.Add(new Vector2(uvBaseX, uvBaseY));
                uvs.Add(new Vector2(uvBaseX + tileSize, uvBaseY));
                uvs.Add(new Vector2(uvBaseX, uvBaseY + tileSize));
                uvs.Add(new Vector2(uvBaseX + tileSize, uvBaseY + tileSize));

                vertIndex += 4;
            }
        }
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        mf.mesh = mesh;
    }
}
