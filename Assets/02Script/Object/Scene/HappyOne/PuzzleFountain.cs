using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Happy NPC
/// </summary>
public class PuzzleFountain : MonoBehaviour
{
    public string npcID;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.PassedQuiz>(OnQuizPass);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.PassedQuiz>(OnQuizPass);
    }

    private void OnQuizPass(PuzzleEvents.PassedQuiz evt)
    {
        if (evt.npcID == npcID)
        {
            Debug.Log("¡§¥‰¿Ãπ«∑Œ ∞≠¡¶ æ∆¿Ã≈€Ω¿µÊ!");

            if (evt.npcID == "N006")
            {
                // æ∆¿Ã≈€ Ω¿µÊ
                EventBus.Instance.Publish<GameEvents.PutItem>(new GameEvents.PutItem(ItemType.Pickable, 10001002));

                // ø≠ºË Ω¿µÊ ¿Ã∫•∆Æ
                EventBus.Instance.Publish<PuzzleEvents.HO_GetKey>(new PuzzleEvents.HO_GetKey());
            }
            else if (evt.npcID == "N007")
            {
                // æ∆¿Ã≈€ Ω¿µÊ
                EventBus.Instance.Publish<GameEvents.PutItem>(new GameEvents.PutItem(ItemType.Pickable, 10001003));
                
                // ø≠ºË Ω¿µÊ ¿Ã∫•∆Æ
                EventBus.Instance.Publish<PuzzleEvents.HO_GetKey>(new PuzzleEvents.HO_GetKey());
            }
            else if (evt.npcID == "N008") {
                // æ∆¿Ã≈€ Ω¿µÊ
                EventBus.Instance.Publish<GameEvents.PutItem>(new GameEvents.PutItem(ItemType.Pickable, 10001004));

                // ø≠ºË Ω¿µÊ ¿Ã∫•∆Æ
                EventBus.Instance.Publish<PuzzleEvents.HO_GetKey>(new PuzzleEvents.HO_GetKey());
            }
        }
    }
}
