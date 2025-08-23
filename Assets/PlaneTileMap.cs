using UnityEngine;

public class PlaneTileMap : MonoBehaviour
{
    public GameObject planePrefab; // 1x1 크기 Plane 프리팹
    public int width = 150;
    public int depth = 100;

    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                GameObject tile = Instantiate(planePrefab, pos, Quaternion.Euler(0, 0, 0));
                tile.transform.parent = this.transform;

                // 필요하면 머티리얼 교체 등 타일별 설정 추가 가능
            }
        }
    }
}
