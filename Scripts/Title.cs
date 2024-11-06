//タイトルの文字を子に持つ空オブジェクトのクラス

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    //タイトルの文字を着色後の色ごとにアタッチ
    [SerializeField] GameObject reds,greens,blues,yellows,cians,magentas;

    int timer;
    Dictionary<string,Color> dic;
    void Start()
    {
        timer=-1;
        SaveData save=General.saveData;
        dic=Config.Title.rgbcmy;

        //その色でクリアしたことがあれば着色
        if(save.red)setColor(reds,dic["red"]);
        if(save.green)setColor(greens,dic["green"]);
        if(save.blue)setColor(blues,dic["blue"]);
        if(save.yellow)setColor(yellows,dic["yellow"]);
        if(save.cian)setColor(cians,dic["cian"]);
        if(save.magenta)setColor(magentas,dic["magenta"]);

        //全ての色でクリアしたことがあれば、色変更用のタイマーを開始
        if(save.red && save.green && save.blue && save.yellow && save.cian && save.magenta){
            timer=0;
        }
    }
    void setColor(GameObject cha,Color col){
        cha.GetComponent<SpriteRenderer>().color=col;
    }

    //赤→黄→緑→水色→青→桃 と色を変える
    void changeColor(GameObject cha){
        var re=cha.GetComponent<SpriteRenderer>();
        Color c=re.color;

        if(c==dic["red"])re.color=dic["yellow"];
        else if(c==dic["yellow"])re.color=dic["green"];
        else if(c==dic["green"])re.color=dic["cian"];
        else if(c==dic["cian"])re.color=dic["blue"];
        else if(c==dic["blue"])re.color=dic["magenta"];
        else if(c==dic["magenta"])re.color=dic["red"];
    }
    void FixedUpdate(){
        if(timer==-1)return;

        timer=(timer+1)%Config.Title.colorInterval;
        if(timer==0){
            changeColor(reds);
            changeColor(greens);
            changeColor(blues);
            changeColor(cians);
            changeColor(magentas);
            changeColor(yellows);
        }
    }
}
