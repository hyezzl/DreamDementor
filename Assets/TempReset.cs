using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempReset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            // 파일 존재할 때만 삭제 (Easy Save 내부적으로 File.Exists 체크)
            if (ES3.FileExists("SaveFile.es3"))
            {
                ES3.DeleteFile("SaveFile.es3");
            }
            else
            {
                Debug.LogWarning("세이브 파일이 없음");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"세이브 파일 삭제 도중 오류 발생! : {ex.Message}");
        }
        ES3.DeleteFile();
    }
}
