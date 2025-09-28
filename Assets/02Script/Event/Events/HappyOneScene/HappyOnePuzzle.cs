using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyOnePuzzle : MonoBehaviour
{
    // 모든게 임시일 것입니다....
    [SerializeField] IActionItem item;
    [SerializeField] GameObject enemy;
    private PlayerController pc;


    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.MakeChoice>(OnChoice);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.MakeChoice>(OnChoice);
    }

    private void OnChoice(UIEvents.MakeChoice evt) {
        if (evt.choiceID == "C005")
        {
            if (evt.selectIdx == 0)
            {
                Debug.Log("정답!");
                // ReDialog

                // 아이템 습득
                //EventBus.Instance.Publish<GameEvents.GetItem>(new GameEvents.GetItem(item));

                // Enemy 활성화
                enemy.SetActive(true);

                // chase 명령
                EventBus.Instance.Publish<GameEvents.EnemyStateChange>(new GameEvents.EnemyStateChange(EnemyState.Chase));

                // 게임모드 변경
                pc.CurMode = GameMode.InspectMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
            }
            else { Debug.Log("오답!"); }
        }
        else if (evt.choiceID == "C006") 
        {
            if (evt.selectIdx == 1)
            {
                Debug.Log("정답!");

            }
            else { Debug.Log("오답!"); }
        } 
        else if(evt.choiceID == "C007")
        {
            if (evt.selectIdx == 0)
            {
                Debug.Log("정답!");

            }
            else { Debug.Log("오답!"); }
        }
    }
}
