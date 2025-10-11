using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerZone
{
    void OnTrigger(GameObject actor);

    void Init(IDatabase db);

    string ZoneID { get; }
}
