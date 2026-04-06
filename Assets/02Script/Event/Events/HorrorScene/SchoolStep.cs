using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolStep : MonoBehaviour
{
    [Header("Area 3: School Loop")]
    public string[] schoolEventIDs;
    private Dictionary<int, DialogData>[] schoolDialogs;


    public void InitData(IDatabase db)
    {
        if (schoolEventIDs != null && schoolEventIDs.Length > 0)
        {
            schoolDialogs = new Dictionary<int, DialogData>[schoolEventIDs.Length];
            for (int i = 0; i < schoolEventIDs.Length; i++)
            {
                schoolDialogs[i] = db.GetDialog(schoolEventIDs[i], schoolEventIDs[i]);
            }
        }
    }
}
