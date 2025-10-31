using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Delete : MonoBehaviour
{
    [SerializeField] private Button btn;

    private void OnEnable()
    {
        btn.onClick.AddListener(DeleteOne);
    }

    private void DeleteOne()
    {
        //StartCoroutine(Dele());
        ES3.DeleteFile("SaveFile.es3", new ES3Settings(ES3.Location.Cache));
    }

    IEnumerator Dele()
    {
        //try
        //{
        //    // 파일 존재할 때만 삭제 (Easy Save 내부적으로 File.Exists 체크)
        //    if (ES3.FileExists("SaveFile.es3"))
        //    {
        //        ES3.DeleteFile("SaveFile.es3");
        //        Debug.Log("세이브 파일이 정상적으로 삭제");
        //    }
        //    else
        //    {
        //        Debug.LogWarning("세이브 파일이 없음");
        //    }
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError($"세이브 파일 삭제 도중 오류 발생! : {ex.Message}");
        //}

        //ES3.DeleteFile();

        //// Easy Save 자동초기화!
        //string filePath = System.IO.Path.Combine(Application.persistentDataPath, "SaveFile.es3");
        //if (System.IO.File.Exists(filePath))
        //    System.IO.File.Delete(filePath);

        string path1 = System.IO.Path.Combine(Application.persistentDataPath, "SaveFile.es3");
        if (System.IO.File.Exists(path1)) System.IO.File.Delete(path1);

        string path2 = System.IO.Path.Combine(Application.persistentDataPath, "ES3AutoSave_Default.es3");
        if (System.IO.File.Exists(path2)) System.IO.File.Delete(path2);

        yield return null;
        yield return null;
        yield return null;
        yield return null;

        Debug.Log("게임 시작!");
        EventBus.Instance.Publish<GameEvents.SwitchScene>(
            new GameEvents.SwitchScene(SceneType.TitleScene, SceneType.TutorialScene));
    }
}
