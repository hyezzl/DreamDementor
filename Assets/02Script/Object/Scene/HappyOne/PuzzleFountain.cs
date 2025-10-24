using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Happy NPC
/// </summary>
public class PuzzleFountain : MonoBehaviour
{
    // 특정 대화가 마무리될때 (맞는 선택지를 골랐을때) AddItem + Key쌓임

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.MakeChoice>(OnChoice);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.MakeChoice>(OnChoice);
    }

    private void OnChoice(UIEvents.MakeChoice evt) {
        if (evt.choiceID == "C005" && evt.isNpc == true)
        {
            if (evt.selectIdx == 0) {
                Debug.Log("정답이쟈나");

                // 아이템 습득
                EventBus.Instance.Publish<GameEvents.PutItem>(new GameEvents.PutItem(ItemType.Pickable, 10001002));
            }
        }

        else if (evt.choiceID == "C006" && evt.isNpc == true)
        {
            if (evt.selectIdx == 1)
            {
                Debug.Log("정답이쟈나");
                // 아이템 습득
                EventBus.Instance.Publish<GameEvents.PutItem>(new GameEvents.PutItem(ItemType.Pickable, 10001003));

            }

        }

        else if (evt.choiceID == "C007" && evt.isNpc == true) {
            if (evt.selectIdx == 0)
            {
                Debug.Log("정답이쟈나");
                // 아이템 습득
                EventBus.Instance.Publish<GameEvents.PutItem>(new GameEvents.PutItem(ItemType.Pickable, 10001004));
            }
        }
    }
}
