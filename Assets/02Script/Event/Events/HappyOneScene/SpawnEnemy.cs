using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{

    [Header("SpawnPoint Location")]
    [SerializeField] private Transform SP01;
    [SerializeField] private Transform SP02;
    [SerializeField] private Transform SP03;

    [Header("Enemy")]
    [SerializeField] private GameObject enemy;


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.HO_AppearEnemy>(OnSpawnEnemy);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.HO_AppearEnemy>(OnSpawnEnemy);
    }

    private void OnSpawnEnemy(PuzzleEvents.HO_AppearEnemy evt) {
        switch (evt.npcID) 
        {
            case "N006":
                Debug.Log("분수 1과 마지막 상호작용");

                // SP01에서 괴물 소환
                enemy.SetActive(true);
                enemy.transform.position = SP01.position;

                break;

            case "N007":
                Debug.Log("분수 2와 마지막 상호작용");

                // SP02에서 괴물 소환
                enemy.SetActive(true);
                enemy.transform.position = SP02.position;
                break;

            case "N008":
                Debug.Log("분수 3과 마지막 상호작용");

                // SP03에서 괴물 소환
                enemy.SetActive(true);
                enemy.transform.position = SP03.position;
                break;

            default:
                return;
        }
        // 소환 직후엔 움직일 수 없음
    }
}


