using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;



public class HideCabinet : MonoBehaviour
{
    private MeshRenderer mr;
    private Animator anim;
    private CharacterController cc;
    private PlayerMove pm;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        cc = GetComponent<CharacterController>();
        pm = GetComponent<PlayerMove>();
        if (!(mr  && cc && pm)) Debug.Log("HideCabinet - Failed to Load something Ref");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.UseCabinet>(OnUseCabinet);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.UseCabinet>(OnUseCabinet);
    }

    private void OnUseCabinet(GameEvents.UseCabinet evt) {
        
        if (evt.isIn)
        {
            // 들어갔을 때
            EnterCabinet(evt.inPos);
        }
        else 
        {
            // 나왔을 때
            ExitCabinet(evt.outPos, evt.outDir);
        }
    }

    private void EnterCabinet(Transform warpInPos) {
        cc.enabled = false;
        
        mr.enabled = false;

        // 위치옮김
        transform.position = warpInPos.position;

        cc.enabled = true;

        // 캐비닛 안에 있을 때는 움직이지 못함
        pm.StopGame();
    }

    private void ExitCabinet(Transform warpOutPos, CabinetDir outDir) {
        cc.enabled = false;

        // 위치 옮김
        transform.position = warpOutPos.position;

        // 다시 움직일 수 있음
        pm.ResumeGame();

        // 바라보는 방향 설정
        Vector2 lookDir = outDir == CabinetDir.Forward ? Vector2.up : Vector2.down;
        if (pm != null) { 
            pm.SetPlayerDir(lookDir);
        }

        mr.enabled = true;
        cc.enabled = true;
    }




}
