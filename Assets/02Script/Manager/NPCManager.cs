using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCManager : MonoBehaviour
{

    private void Start()
    {
        SyncNPCState();
    }


    public void SyncNPCState()
    {
        var allNpcs = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, 
                FindObjectsSortMode.None).OfType<IActionNpc>();

        foreach (var npcInt in allNpcs)
        {
            string id = npcInt.GetNpcID();

            bool alreadyMet = EventHistoryManager.Instance.IsEventComplete(id);

            // NPC isContact °»½Å
            if (npcInt is NPC npc)
            {
                npc.isContacted = alreadyMet;
            }
        }
    }
}
