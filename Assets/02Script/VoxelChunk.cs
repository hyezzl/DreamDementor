using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelChunk : MonoBehaviour
{
    public int chunkSize = 16;
    public float voxelSize = 1f;
    private int[,,] voxels;

    void Start()
    {
        // 1. 청크 내 복셀 데이터 초기화 (절반은 블록, 절반은 빈칸)
        voxels = new int[chunkSize, chunkSize, chunkSize];
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (y < chunkSize / 2) voxels[x, y, z] = 1;
                    else voxels[x, y, z] = 0;
                }
            }
        }

        // 2. 복셀 데이터 기준으로 큐브 생성하기
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (voxels[x, y, z] == 1)
                    {
                        Vector3 pos = new Vector3(x * voxelSize, y * voxelSize, z * voxelSize);
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = pos;
                        cube.transform.parent = this.transform;
                        cube.transform.localScale = Vector3.one * voxelSize;
                    }
                }
            }
        }
    }
}

