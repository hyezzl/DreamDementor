using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderData;

/// <summary>
/// 선택지 따라 적대적 / 친화적 나뉘는 NPC
/// </summary>

public class NeutralNPC : NPC
{
    public bool isFriend = false;       // 친화적인지

    public override void OnNpcChoice(UIEvents.MakeChoice evt)
    {
        if (evt.isNpc)
        {
            // ChoiceData에서 score꺼내어 값이 0이면 정답처리
            int score;
            if (evt.data != null)
            {
                score = evt.data.scores[evt.selectIdx];
                if (score == 0)
                {
                    // 값이 0이면(정답이면 친화적)
                    isFriend = true;
                }
            }
        }

        base.OnNpcChoice(evt);
    }
}
