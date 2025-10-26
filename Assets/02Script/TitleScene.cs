using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button startBTN;
    [SerializeField] private Image start;
    [SerializeField] private Button loadBTN;
    [SerializeField] private Image load;
    [SerializeField] private Button exitBTN;
    [SerializeField] private Image exit;


    private Image curImg;       // 현재 활성화된 이미지
    private void Awake()
    {
        // 초기화
        start.enabled = false;
        load.enabled = false;
        exit.enabled = false;
    }

    private void OnEnable()
    {
        startBTN.onClick.AddListener(() => OnButtonClick(start));
        loadBTN.onClick.AddListener(() => OnButtonClick(load));
        exitBTN.onClick.AddListener(() => OnButtonClick(exit));
    }
    private void OnDisable()
    {
        startBTN.onClick.RemoveAllListeners();
        loadBTN.onClick.RemoveAllListeners();
        exitBTN.onClick.RemoveAllListeners();
    }


    private void OnButtonClick(Image selected)
    {
        // 클릭된 버튼을 활성화하고, 나머지를 비활성화
        OneOnly(selected);

        if (selected == start)
        {
            Debug.Log("게임 시작!");
            EventBus.Instance.Publish<GameEvents.SwitchScene>(new GameEvents.SwitchScene(SceneType.TutorialScene));
        }
        else if (selected == load)
        {
            Debug.Log("불러오기 버튼 클릭");
            // 로드 관련 로직 추가 가능
        }
        else if (selected == exit)
        {
            Debug.Log("게임 종료!");
            Application.Quit();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject focusObj = eventData.pointerEnter;

        if (focusObj == startBTN.gameObject)
            start.enabled = true;
        else if(focusObj == loadBTN.gameObject)
            load.enabled = true;
        else if(focusObj == exitBTN.gameObject)
            exit.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameObject focusObj = eventData.pointerEnter;

        if (focusObj == startBTN.gameObject && curImg != start)
            start.enabled = false;
        else if(focusObj == loadBTN.gameObject && curImg != load)
            load.enabled = false;
        else if(focusObj == exitBTN.gameObject && curImg != exit)
            exit.enabled = false;
    }

    private void OneOnly(Image target) {
        start.enabled = false;
        load.enabled = false;
        exit.enabled = false;

        target.enabled = true;
        curImg = target;
    }
}
