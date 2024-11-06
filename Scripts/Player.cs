//プレイヤーの魚のクラス

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Player : MonoBehaviour
{
    Vector3 speed;
    float powerRight,powerLeft,powerUp,powerDown;
    public static Vector3 scale;
    public static Color col;
    General general;
    void Start()
    {
        speed = Vector3.zero;
        powerLeft=1.0f;
        powerRight=1.0f;
        powerUp=1.0f;
        powerDown=1.0f;
        scale = new Vector3(0.1f,0.1f, 0.1f);
        col=new Color(0,0,0);

        gameObject.GetComponent<SpriteRenderer>().color = col;
        general=GameObject.Find("General").GetComponent<General>();
    }
    void FixedUpdate()
    {
        bool up=false,down=false,right=false,left=false;
        
        //キーボード入力処理
        if (Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow))
        {
            up=true;
        }
        if (Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow))
        {
            if (scale.x < 0) scale.x *= -1;
            left=true;
        }
        if (Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow))
        {
            down=true;
        }
        if (Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow))
        {
            if (scale.x > 0) scale.x *= -1;
            right=true;
        }
        /*
        if(Input.GetKey(KeyCode.Q)){
                result = "clear";
                SceneManager.LoadScene("ResultScene");
        }
        */

        //移動の度合いの計算
        if(right)powerRight=accelPower(powerRight);
        else powerRight=brakePower(powerRight);
        if(left)powerLeft=accelPower(powerLeft);
        else powerLeft=brakePower(powerLeft);
        if(up)powerUp=accelPower(powerUp);
        else powerUp=brakePower(powerUp);
        if(down)powerDown=accelPower(powerDown);
        else powerDown=brakePower(powerDown);

        speed.x=powerRight-powerLeft;
        speed.y=powerUp-powerDown;

        //画面から出ないようにする
        var w=Config.width;
        var h=Config.height;
        var posx=transform.position.x+speed.x;
        var posy=transform.position.y+speed.y;
        if(posx>w/2)speed.x=w/2-transform.position.x;
        if(posx<-w/2)speed.x=-w/2-transform.position.x;
        if(posy>h/2)speed.y=h/2-transform.position.y;
        if(posy<-h/2)speed.y=-h/2-transform.position.y;

        //移動など反映
        transform.Translate(speed);
        transform.localScale = scale;
    }

    //加速・減速
    float accelPower(float p){
        p*=Config.Player.accel;
        if(p>Config.Player.maxSpeed)p=Config.Player.maxSpeed;
        return p;
    }
    float brakePower(float p){
        p*=Config.Player.brake;
        if(p<1)p=1.0f;
        return p;
    }

    //当たり判定・敵を食べるメソッド
    void OnTriggerEnter2D(Collider2D other) {
        string name=other.gameObject.name;
        if(name!="enemy(Clone)")return;

        float siz = other.gameObject.transform.localScale.y;
        if (scale.y+Config.eatOverScale < siz)return;

        eatEnemy(other.gameObject,siz);
    }
    void eatEnemy(GameObject enemy,float siz){
        //食べるSE
        if (Random.Range(0, 2) == 0)playOneShotSelf(general.as_eat1);
        else playOneShotSelf(general.as_eat2);

        //プレイヤーのサイズ・色変化を計算
        if(enemy.tag=="enemy_gaming"){
            General.eaten_gamingfish++;
        }else{
            float pc = siz/scale.y;
            float bigger = pc*Config.Player.growRatio+1;
            scale *= bigger;
            
            Color c = enemy.GetComponent<SpriteRenderer>().color;

            if (c.r == 1)col.r=incrementPrimaryColor(col.r);
            else if (c.g == 1)col.g=incrementPrimaryColor(col.g);
            else if (c.b == 1)col.b=incrementPrimaryColor(col.b);

            gameObject.GetComponent<SpriteRenderer>().color=col;
        }
        Destroy(enemy);
        
        //クリアの場合
        if (scale.y > Config.Player.maxScale)
        {
            disableCollider();
            var script=GameObject.Find("General").GetComponent<General>();
            script.StartCoroutine("clear");
        }
    }

    //プレイヤーの色のRかGかBを増やす
    float incrementPrimaryColor(float c){
        c+=Config.Player.changeColorBy;
        if(c>1)c=1.0f;
        return c;
    }

    //当たり判定消去
    void disableCollider(){
        GetComponent<PolygonCollider2D>().enabled=false;
        GameObject.Find("PlayerInnerCol").GetComponent<CapsuleCollider2D>().enabled=false;
    }

    //AudioSource変数の名前が長くてかさばったためメソッド化
    void playOneShotSelf(AudioSource a){
        a.PlayOneShot(a.clip);
    }

}

