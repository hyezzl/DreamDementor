using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizNPC : NPC
{
    public bool isPass = false;     // 퀴즈 통과 여부

    public override void Interact()
    {
        if (isInDialog || isDelay) return; // 대화중이거나, 딜레이 중이면 무시

        if (!isPass)
        {
            // 퀴즈 통과 전 : 대화
            isInDialog = true;
            EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, initialDialog));
            //isContacted = true;
        }
        else
        {
            // 퀴즈 통과 후 재대화
            isInDialog = true;
            EventBus.Instance.Publish<UIEvents.OpenNpcReDialog>(new UIEvents.OpenNpcReDialog(npcID, reDialogData));
        }
    }

    public override void OnNpcChoice(UIEvents.MakeChoice evt)
    {
        if (evt.isNpc)
        {
            // ChoiceData에서 score꺼내어 값이 0이면 정답처리
            int score;
            if (evt.data != null)
            {
                score = evt.data.scores[evt.selectIdx];
                if (score == 0) {
                    // 값이 0이면
                    isPass = true;
                }
            }
        }

        base.OnNpcChoice(evt);
    }
}
