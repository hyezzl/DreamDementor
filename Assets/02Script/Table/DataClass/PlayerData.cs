using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public Dictionary<Stage, int> playerHP;     //  스테이지당 HP
    public SceneType scene;                     // 플레이어의 현재 씬
    public List<ItemInstance> inventory;        // 플레이어의 인벤토리
    public List<string> completedEvents = new();    // 실행된 이벤트
    public Dictionary<SceneType, bool> visitScene = new();     // 씬 방문 데이터
    public Dictionary<string, bool> zoneActiveMap = new();      // 존 활성/비활성 데이터
}


