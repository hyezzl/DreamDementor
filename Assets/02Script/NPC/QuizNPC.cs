using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizNPC : NPC
{
    public bool isPass = false;     // 퀴즈 통과 여부
    public bool isDeactive = false;

    public override void Interact()
    {
        if (isInDialog || isDelay) return; // 대화중이거나, 딜레이 중이면 무시

        if (!isPass)
        {
            // 퀴즈 통과 전 : 대화
            isInDialog = true;
            EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, npcID, initialDialog));
            isContacted = true;
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
        if (evt.isNpc && evt.data.rootID == npcID)
        {
            // ChoiceData에서 score꺼내어 값이 0이면 정답처리
            int score;
            if (evt.data != null)
            {
                score = evt.data.scores[evt.selectIdx];
                if (score == 0)
                {
                    // 값이 0이면
                    isPass = true;

                    // 퀴즈 합격
                    //EventBus.Instance.Publish<PuzzleEvents.PassedQuiz>(new PuzzleEvents.PassedQuiz(npcID));
                }
            }
        }

        base.OnNpcChoice(evt);
    }

    // 패스되고 난 후, 대화이벤트 끝났을 때
    protected override void OnEndNpcDialog(UIEvents.EndNpcDialog evt)
    {
        // 통과 이후 대화끝
        if (evt.npcID == npcID && evt.npcEventID == curNpcEventID && isPass)
        {
            if (!isDeactive) { 
                // 퀴즈 합격 (단발)
                EventBus.Instance.Publish<PuzzleEvents.PassedQuiz>(new PuzzleEvents.PassedQuiz(npcID));
                isDeactive = true;
            }
        }
        // 통과 이전 대화끝
        else if(evt.npcID == npcID && evt.npcEventID == curNpcEventID && !isPass)
        {
            Debug.Log($"*통과 이전 대화끝입니다");
        }
        base.OnEndNpcDialog(evt);
    }
}