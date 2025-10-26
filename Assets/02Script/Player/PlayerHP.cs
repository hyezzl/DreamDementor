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
        EventBus.Instance.Subscribe<GameEvents.SceneStart>(OnSceneStart);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.OnHpChange>(OnHpChange);
        EventBus.Instance.Unsubscribe<GameEvents.SceneStart>(OnSceneStart);
    }

    private void OnSceneStart(GameEvents.SceneStart evt) {
        //씬이 새로 시작되면 이전 값 업데이트
        int curHP = PlayerController.Instance.GetCurHP(evt.curScene);
        Debug.Log($"씬시작! : 기존 HP : {curHP}");

        ChangeMoon(curHP);
    }

    private void OnHpChange(GameEvents.OnHpChange evt) {
        int curHP = Mathf.Clamp(evt.newHp, 0, 100);
        Debug.Log($"현재 HP : {curHP}");
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
