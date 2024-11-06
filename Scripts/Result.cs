//スコア表示画面のスクリプト(空オブジェクトのクラス)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class Result : MonoBehaviour
{
    //SE、各種テキストUIをアタッチ
    [SerializeField] private AudioSource as_clear,as_out,as_result,as_newRecord;
    [SerializeField] GameObject gaming,massaget,calct,resultt,newrecordt,againt;

    void Start()
    {
        //ゲーム開始前だった場合はタイトル画面に切り替え
        if(General.result==null){
            SceneManager.LoadScene("MainScene");
            return;
        }
        
        resultt.GetComponent<Text>().text="";
        calct.GetComponent<Text>().text="";
        newrecordt.SetActive(false);
        againt.SetActive(false);
        gaming.SetActive(false);

        if (General.result=="clear"){
            showScore();
        }else {
            showGameover();
        }

        calct.GetComponent<Text>().color=Player.col;
    }

    //ゲームオーバーの場合
    void showGameover()
    {
        as_out.PlayOneShot(as_out.clip);
        massaget.GetComponent<Text>().text = "GAME OVER";
        calct.GetComponent<Text>().text = "";
        resultt.GetComponent<Text>().text = "";
        againt.SetActive(true);
        Destroy(gaming);
    }

    //クリアの場合
    void showScore()
    {
        massaget.GetComponent<Text>().text = "CONGRATULATIONS!";
        as_clear.PlayOneShot(as_clear.clip);

        //プレイヤーの色を判定
        Color c=Player.col;
        float bias=Config.pickyBias;
        ArrayList colorBonus;

        if(c.r>c.g*bias && c.r>c.b*bias){
            colorBonus=Config.ColorBonus.red;
            General.saveData.red=true;
        }
        else if(c.g>c.r*bias && c.g>c.b*bias){
            colorBonus=Config.ColorBonus.green;
            General.saveData.green=true;
        }
        else if(c.b>c.r*bias && c.b>c.g*bias){
            colorBonus=Config.ColorBonus.blue;
            General.saveData.blue=true;
        }
        else if(c.r>c.b*bias && c.g>c.b*bias){
            colorBonus=Config.ColorBonus.yellow;
            General.saveData.yellow=true;
        }
        else if(c.r>c.g*bias && c.b>c.g*bias){
            colorBonus=Config.ColorBonus.magenta;
            General.saveData.magenta=true;
        }
        else if(c.g>c.r*bias && c.b>c.r*bias){
            colorBonus=Config.ColorBonus.cian;
            General.saveData.cian=true;
        }
        else colorBonus=Config.ColorBonus.not;
        
        //スコア計算
        float gamingBonus=Config.GamingFish.scoreBonus*General.eaten_gamingfish;
        int finalScore=(int)Math.Ceiling(General.score*((int)colorBonus[1]+gamingBonus));

        //スコアのテキストを時間差で少しずつ表示
        Action[] actions=new Action[5]{
            ()=>{
                as_result.PlayOneShot(as_result.clip);
                calct.GetComponent<Text>().text="TIME BONUS: "+General.score;
            },
            ()=>{
                as_result.PlayOneShot(as_result.clip);
                calct.GetComponent<Text>().text+="\n"+(string)colorBonus[0]+": ×"+(int)colorBonus[1];
            },
            ()=>{
                as_result.PlayOneShot(as_result.clip);
                gaming.SetActive(true);
                calct.GetComponent<Text>().text+="\n                       : ×"+(1.0f+gamingBonus);
            },
            ()=>{
                
            },
            ()=>{
                againt.SetActive(true);
            }
        };

        //記録更新した場合は"newRecord"表示
        if(finalScore>General.saveData.bestScore){
            General.saveData.bestScore=finalScore;
            General.saveData.bestColor=c;
            actions[3]=()=>{
                as_newRecord.PlayOneShot(as_newRecord.clip);
                newrecordt.SetActive(true);
                resultt.GetComponent<Text>().text+="SCORE:"+finalScore;
            };
        }else{
            actions[3]=()=>{
                as_result.PlayOneShot(as_result.clip);
                resultt.GetComponent<Text>().text+="SCORE:"+finalScore;
            };
        }
    
        multiDelayActions(0.6f,actions);
        
        //保存
        string json = JsonUtility.ToJson(General.saveData);
        File.WriteAllText(Config.savePath, json);
    }

    //時間差で複数のメソッドを動かす ためのメソッド
    void multiDelayActions(float wait,Action[] actions){
        foreach(var (action, i) in actions.Select((value, index) => (value, index))){
            StartCoroutine(manageDelay(wait*(i+1),action));
        }
    }
    IEnumerator manageDelay(float wait,Action action){
        yield return new WaitForSeconds(wait);
            action();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene("MainScene");
        }
        if(Input.GetKey(KeyCode.Space)){
            General.result=null;
            SceneManager.LoadScene("MainScene");
        }
    }
    
}
