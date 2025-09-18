using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private Button startBTN;
    [SerializeField] private Button loadBTN;
    [SerializeField] private Button quitBTN;

    private void OnEnable()
    {
        startBTN.onClick.AddListener(ConvertTitle2Tutorial);
    }
    private void OnDisable()
    {
        startBTN.onClick.RemoveAllListeners();
    }


    public void ConvertTitle2Tutorial()
    {
        //PlayerPrefs.SetString("StartNext", "First"); // 로딩 씬 필요할 시
        SceneManager.LoadScene("TutorialScene");
    }
}
