using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleBTN : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image highlight;              // 버튼 하이라이트 이미지
    [SerializeField] private TitleBTN[] otherButtons;   // 같은 그룹의 버튼들
    private bool isSelected = false;

    private void Awake()
    {
        if (highlight != null)
            highlight.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && highlight != null)
            highlight.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected && highlight != null)
            highlight.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭 시 자신만 활성화 + 나머지는 비활성화
        foreach (var btn in otherButtons)
        {
            btn.Deselect();
        }

        Select();

        // 버튼별 행동 정의
        if (name.Contains("Start"))
        {
            // 1. 세이브 데이터 삭제
            if (ES3.KeyExists("GameSave"))
            {
                ES3.DeleteKey("GameSave");
                Debug.Log("세이브 데이터가 초기화!");
            }

            // ES3 파일 직접 삭제!
            //ES3.DeleteFile("SaveFile.es3");
            //Debug.Log("세이브파일 자체가 완전히 삭제");

            ES3.DeleteFile();

            Debug.Log("게임 시작!");
            EventBus.Instance.Publish<GameEvents.SwitchScene>(
                new GameEvents.SwitchScene(SceneType.TitleScene, SceneType.TutorialScene));
        }
        else if (name.Contains("Load"))
        {
            // 1. 세이브 파일이 있는지 체크!
            if (ES3.KeyExists("GameSave"))
            {
                // 2. 세이브 데이터에서 씬 이름 불러오기
                var saveData = ES3.Load<SaveData>("GameSave");
                string sceneName = saveData.scene.ToString(); // enum이면 ToString(), string이면 그대로

                // 3. 해당 씬으로 이동!
                EventBus.Instance.Publish<GameEvents.SwitchScene>(
                    new GameEvents.SwitchScene(SceneType.TitleScene, SceneType.TutorialScene));

                Debug.Log("저장된 데이터로 불러오기 성공!");
            }
            else
            {
                Debug.LogWarning("저장된 데이터가 없어요! 새로 시작해야 합니다.");
                // 원하면 안내 메시지 띄우기!
            }
            
        }
        else if (name.Contains("Exit"))
        {
            Debug.Log("게임 종료!");
            Application.Quit();
        }
    }

    private void Select()
    {
        isSelected = true;
        if (highlight != null)
            highlight.enabled = true;
    }

    public void Deselect()
    {
        isSelected = false;
        if (highlight != null)
            highlight.enabled = false;
    }
}
