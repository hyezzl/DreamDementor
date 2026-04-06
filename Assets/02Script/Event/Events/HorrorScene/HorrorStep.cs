using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorStep : MonoBehaviour
{
    protected IDatabase cacheDB;

    public virtual void InitData(IDatabase db)
    {
        cacheDB = db;
    }
}
