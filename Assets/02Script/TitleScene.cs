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
        startBTN.onClick.AddListener(GameStart);
        loadBTN.onClick.AddListener(LoadGame);
        quitBTN.onClick.AddListener(QuitGame);
    }
    private void OnDisable()
    {
        startBTN.onClick.RemoveAllListeners();
        loadBTN.onClick.RemoveAllListeners();
        quitBTN.onClick.RemoveAllListeners();
    }


    private void GameStart() {
        // 씬이동
        EventBus.Instance.Publish<GameEvents.SwitchScene>(new GameEvents.SwitchScene(SceneType.TutorialScene));
    }

    private void LoadGame() { 
        
    }

    private void QuitGame() {
        Debug.Log("게임 종료!");
        Application.Quit();
    }
}
