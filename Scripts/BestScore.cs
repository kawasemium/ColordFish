//タイトル画面左のベストスコアを表示するテキストUIのクラス

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestScore : ButtonsOnTitle
{
    //このボタンを踏んだときの音を変えたかったのでアタッチ
    [SerializeField] AudioSource as_result;
    
    void Start()
    {
        base.init();
        GetComponent<SpriteRenderer>().color=General.saveData.bestColor;

        //記録がない場合は表示しない
        if(General.saveData.bestScore==0 || General.result!=null){
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if(!General.playing)return;

        //ゲームが始まっている場合は左に捌ける
        Vector3 scl=transform.localScale;
        if(scl.x<0)transform.localScale=new Vector3(-scl.x,scl.y,scl.z);
        transform.Translate(-Config.Enemy.maxSpeed, 0, 0);
        float dst = Mathf.Abs(transform.position.x);
        if (dst > Config.width/2+5) Destroy(gameObject);
    }

    protected override void pushed(){
        as_result.PlayOneShot(as_result.clip);
    }
    protected override void pushend(){

    }
}
