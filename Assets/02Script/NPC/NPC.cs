using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IActionNpc
{
    public int NPCID;
    private NPCDialogData dialogData;
    private NPCReDialogData reDialogData;
    private IDatabase database;

    public int GetNpcID() => NPCID;

    public void Init(IDatabase db)
    {
        database = db;
    }

    public void Interact()
    {
        //
    }
}
