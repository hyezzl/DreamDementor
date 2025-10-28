using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int playerHP;
    public List<ItemInstance> inventory;
    public List<string> completedEvents;
    //public Dictionary<SceneType, bool>
}
