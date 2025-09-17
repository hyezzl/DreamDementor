using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sr;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.UpdateEnemy>(OnStateChange);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.UpdateEnemy>(OnStateChange);
    }

    private void OnStateChange(GameEvents.UpdateEnemy evt) { 

    }


}
