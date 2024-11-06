//敵の魚のプレハブが持つクラス

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public static Color col;
    Vector3 moveto,scl;
    AudioSource as_gamingfish;
    SpriteRenderer spriteRenderer;
    int gamingProgress;

    void Start()
    {
        GameObject g=GameObject.Find("General");
        if(g!=null)as_gamingfish=g.GetComponent<General>().as_gamingFish;
        spriteRenderer=GetComponent<SpriteRenderer>();
        
        //リザルト画面の場合は何もしない
        if(this.tag=="result_gaming"){
            moveto=Vector3.zero;
            return;
        }

        //大きさ決定
        float Mscl=Config.Enemy.maxScale;
        float mscl=Config.Enemy.minScale;
        if(General.clearRatio<Config.Enemy.halfMaxScaleUntil)Mscl/=2;
        float siz =Random.value *(Mscl-mscl)+mscl;
        scl = new Vector3(siz,siz,siz);
        
        //スピード決定
        float maxs = Config.Enemy.maxSpeed;
        float mins = Config.Enemy.minSpeed;
        float go = Random.value * maxs + mins;

        //確率で虹色の魚に変化
        int until=Config.GamingFish.clearRatioUntil;
        int bottom=Config.GamingFish.appearRatioBottom;
        if(General.clearRatio<until && Random.Range(0,bottom)==0){
            float s=Config.GamingFish.scale;
            scl=new Vector3(s,s,s);
            go=Config.GamingFish.speed;
            this.tag="enemy_gaming";
            gamingProgress=0;
            if(g!=null)as_gamingfish.PlayOneShot(as_gamingfish.clip);
            //Debug.Log("gaming fish appeared");
        }

        //向き決定
        if (Random.Range(0, 2) == 0) go *= -1; else scl.x *= -1;
        moveto = new Vector3(go, 0, 0);
        
        //スポーン位置決定
        Vector3 spos;
        spos.z = 0;
        spos.y = Random.value * Config.height-Config.height/2;
        spos.x = Config.width/2+3;
        if (go > 0) spos.x *= -1;

        //色決定
        int cran = Random.Range(0, 3);
        col=Config.Enemy.RGBColors[cran];
        if(this.tag=="enemy_gaming" || this.tag=="result_gaming"){
            col=Color.red;
        }
        spriteRenderer.color = col;
        transform.position = spos;
        transform.localScale = scl;

    }

    void FixedUpdate()
    {
        transform.Translate(moveto);
        float dst = Mathf.Abs(transform.position.x);
        if (dst > Config.width/2+5) Destroy(gameObject);

        //虹色の魚なら色を変更
        if(this.tag=="enemy_gaming" || this.tag=="result_gaming"){
            changeColor();
        }
    }

    void changeColor(){
        float ch=Config.GamingFish.colorChangeSpeed;
        switch(gamingProgress)
        {
            case 0:
                col.g+=ch;
                if(col.g>=1){
                    gamingProgress++;
                }
                break;
            case 1:
                col.r-=ch;
                if(col.r<=0)gamingProgress++;
                break;
            case 2:
                col.b+=ch;
                if(col.b>=1)gamingProgress++;
                break;
            case 3:
                col.g-=ch;
                if(col.g<=0)gamingProgress++;
                break;
            case 4:
                col.r+=ch;
                if(col.r>=1)gamingProgress++;
                break;
            case 5:
                col.b-=ch;
                if(col.b<=0)gamingProgress=0;
                break;
            default:
                break;
        }
        spriteRenderer.color=col;
    }
}
