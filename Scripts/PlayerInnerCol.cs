//プレイヤーの魚内部のやられ専用当たり判定のクラス

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInnerCol : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.name!="enemy(Clone)")return;

        //プレイヤーよりも大きい敵に当たったらミス
        float siz = other.gameObject.transform.localScale.y;
        if (Player.scale.y+Config.eatOverScale < siz)
        {
            var script=GameObject.Find("General").GetComponent<General>();
            script.StartCoroutine("miss");
        }
    }
}
