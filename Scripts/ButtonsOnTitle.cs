//タイトル画面に設置したボタンの共通クラス

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsOnTitle : MonoBehaviour
{
    //このボタンを踏むとアクティブ化するオブジェクトやスライダーがあればアタッチ
    [SerializeField] GameObject activateObject,slider;
    
    AudioSource as_buttonDown,as_buttonUp,as_activateObject;
    bool playerStaying;
    protected Color32 pushingColor;
    Slider comp_slider;
    void Start()
    {
        init();
    }
    protected void init(){
        playerStaying=false;

        var script=GameObject.Find("General").GetComponent<General>();
        as_buttonDown=script.as_buttonDown;
        as_buttonUp=script.as_buttonUp;
        as_activateObject=script.as_activateObject;
        pushingColor=new Color32(150,150,200,255);

        if(slider!=null){
            comp_slider=slider.GetComponent<Slider>();
            comp_slider.value=0;
            slider.SetActive(false);
        }
        if(activateObject!=null)activateObject.SetActive(false);

    }

    void FixedUpdate()
    {
        if(slider==null)return;

        //スライダーを持つ場合は進行させる(ゲーム開始)
        if(slider.activeSelf){
            comp_slider.value++;

            if(comp_slider.value>=Config.Title.delayPlay){
                GameObject.Find("General").GetComponent<General>().playGame();
            }
        }
    }

    //トリガー
    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.name!="Player")return;
        if(playerStaying)return;

        playerStaying=true;
        if(activateObject!=null)activateObject.SetActive(true);
        if(slider!=null)slider.SetActive(true);
        pushed();
    }
    void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.name!="Player")return;
        if(!playerStaying)return;

        playerStaying=false;
        if(activateObject!=null)activateObject.SetActive(false);
        if(comp_slider!=null){
            comp_slider.value=0;
            slider.SetActive(false);
        }
        pushend();
    }

    //ボタンを押した・離した際の追加処理 オーバーライド用
    virtual protected void pushed(){
        as_buttonDown.PlayOneShot(as_buttonDown.clip);
        if(activateObject)as_activateObject.PlayOneShot(as_activateObject.clip);
        GetComponent<SpriteRenderer>().color=pushingColor;
    }
    virtual protected void pushend(){
        as_buttonUp.PlayOneShot(as_buttonUp.clip);
        GetComponent<SpriteRenderer>().color=new Color(1,1,1);
    }
}
