using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionTarget
{
    void Init(IDatabase db);

    void Interact();

    GameObject GetObject();
}
