using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [Header("UI Ref")]
    [SerializeField] private Image img;

    public Sprite[] moons;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.OnHpChange>(OnHpChange);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.OnHpChange>(OnHpChange);
    }

    private void OnHpChange(GameEvents.OnHpChange evt) {
        int curHP = Mathf.Clamp(evt.newHp, 0, 100);
        ChangeMoon(curHP);
    }

    private void ChangeMoon(int curHP) {
        // 10단위로 나누어 계산
        int idx = curHP / 10;
        if (idx < 0) idx = 0;
        if(idx >= moons.Length) idx = moons.Length - 1;

        if (img != null) { 
            img.sprite = moons[idx];
        }
    }
}
