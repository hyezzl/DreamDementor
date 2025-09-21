using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private SceneType curscene;

    private PlayerController pc;
    private CharacterController cc;
    //private bool isDead = false;  // 플레이어 쥬금



    private void Awake()
    {
        if (!TryGetComponent<CharacterController>(out cc))
            Debug.Log("GameOver - Failed to Load CharacterController");
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("GameOver - Failed to Load PlayerController");
    }


}
