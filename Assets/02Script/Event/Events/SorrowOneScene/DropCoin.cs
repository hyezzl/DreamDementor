using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCoin : MonoBehaviour
{
    public Animator dropAnim;
    private Vector2 pos;

    private void Start()
    {
        pos = transform.position;
    }






    // 오답일 때 위치 복구
    public void ReturnCoin() { 
        
    }
}
