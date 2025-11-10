using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineToLinePuzzle : MonoBehaviour
{
    public Transform[] bottomPoints;    // 5, 6, 7, 8
    public Transform[] topPoints;       // 1, 2, 3, 4
    public GameObject[] lineSprites;
    public GameObject[] fixedLines;     // 정답 시 나올 선

    private int curStartIdx = -1;       // 드래그 시작점 Index

    private void Start()
    {
        // 초기값
        foreach (var line in lineSprites) line.SetActive(false);
        foreach(var line in fixedLines) line.SetActive(false);
    }

}
