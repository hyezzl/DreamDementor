using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateStatus : MonoBehaviour
{
    [Header("UI Ref")]
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image faceImg;
    [SerializeField] private Sprite[] faces;
    private int curHP;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.SceneStart>(OnSceneStart);
        EventBus.Instance.Subscribe<GameEvents.OnHpChange>(OnHpChange);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.OnHpChange>(OnHpChange);
        EventBus.Instance.Unsubscribe<GameEvents.SceneStart>(OnSceneStart);
    }

    private void OnHpChange(GameEvents.OnHpChange evt) 
    {
        // Hp 변경될 때 업데이트
        curHP = Mathf.Clamp(evt.newHp, 0, 100);
        UpdateHP();
        UpdateFace();
    }

    private void OnSceneStart(GameEvents.SceneStart evt) 
    {
        curHP = PlayerController.Instance.CurHP;
        UpdateHP();
        UpdateFace();
    }

    private void UpdateHP() {
        if (bar != null && text != null)
        {
            bar.fillAmount = (float)curHP / 100;
            text.text = curHP.ToString() + " / 100";
        }
    }

    private void UpdateFace() {
        if (faceImg != null || curHP <= 0 || faces != null) {
            if (curHP > 0 && curHP <= 30)
            {
                faceImg.sprite = faces[0];
            }
            else if (curHP > 30 && curHP <= 60)
            {
                faceImg.sprite = faces[1];
            }
            else if (curHP > 60 && curHP <= 100) 
            {
                faceImg.sprite = faces[2];
            }
        }
    }
}
