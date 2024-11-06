/*==============================

ゲーム設定のクラスConfig、
タイトル画面・ゲーム中に動くスクリプト(空オブジェクトのクラス)、
セーブデータのクラス

==============================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//ゲーム設定
public static class Config{
    public static readonly float width=18.0f;//画面サイズ
    public static readonly float height=10.0f;

    public static readonly string savePath="./save.json";

    public static readonly int initialTimeBonus=30000;//タイムボーナス初期値
    public static readonly float pickyBias=3.0f;//偏食判定のcolor閾値(多い色/その他の色)
    public static readonly float eatOverScale=0.015f;//自分より大きい魚も食べられる誤差
    
    //プレイヤー
    public static class Player{
        public static readonly float changeColorBy=0.045f;
        public static readonly float growRatio=0.12f;
        public static readonly float accel=1.00017f;//加速時スピード倍率
        public static readonly float brake=0.9998f;//減速時スピード倍率
        public static readonly float maxSpeed=1.045f;
        public static readonly float maxScale=1.0f;
        public static readonly float initialScale=0.1f;
    }

    //敵
    public static class Enemy{
        public static readonly int initialNum=6;//一番最初に一気に出てくる数
        public static readonly int interval=130;//敵生成間隔
        public static readonly float maxSpeed=0.013f;
        public static readonly float minSpeed=0.008f;
        public static readonly float maxScale=0.5f;
        public static readonly float minScale=0.04f;
        public static readonly int halfMaxScaleUntil=10;//クリア率がこの値になるまでは敵の最大サイズが半減する
        public static readonly Color[] RGBColors=new Color[3]{
            new Color(1,0,0),
            new Color(0,1,0),
            new Color(0,0,1),
        };
    }

    //虹色の魚
    public static class GamingFish{
        public static readonly float scoreBonus=0.5f;//一匹当たりのスコア加算倍率
        public static readonly int clearRatioUntil=60;//クリア率がこの値になるまでの間のみ出現する
        public static readonly int appearRatioBottom=30;//出現率。1/母数
        public static readonly float colorChangeSpeed=0.04f;
        public static readonly float speed=0.027f;
        public static readonly float scale=0.04f;
    }

    //偏食ボーナス設定 {"表示テキスト", 点数倍率}
    public static class ColorBonus{
        public static readonly ArrayList red=new ArrayList{"Red Lover      ",5};
        public static readonly ArrayList green=new ArrayList{"Green Lover  ",5};
        public static readonly ArrayList blue=new ArrayList{"Blue Lover     ",5};
        public static readonly ArrayList yellow=new ArrayList{"Yellow Lover  ",3};
        public static readonly ArrayList magenta=new ArrayList{"Magenta Lover",3};
        public static readonly ArrayList cian=new ArrayList{"Cian Lover     ",3};
        public static readonly ArrayList not=new ArrayList{"Not Picky       ",1};

    }

    //タイトル画面の設定
    public static class Title{
        public static readonly int delayPlay=300;//スタートボタンに乗る時間
        public static readonly int colorInterval=100;//タイトルの色付き文字の色変え間隔
        public static readonly Dictionary<string,Color> rgbcmy =new Dictionary<string, Color>{
            //タイトルの文字の色の付け方
            {"red",new Color(1,0.6f,0.6f)},
            {"yellow",new Color(0.9f,0.9f,0.5f)},
            {"green",new Color(0.5f,1,0.5f)},
            {"cian",new Color(0.5f,0.95f,0.95f)},
            {"blue",new Color(0.7f,0.7f,1)},
            {"magenta",new Color(1,0.5f,1)}
        };
    }

}


//空オブジェクトのクラス
public class General : MonoBehaviour
{
    //敵のプレハブ、BGM、SEをアタッチ
    [SerializeField] GameObject enemy_prefab;
    [SerializeField] AudioSource as_GlassSnow,as_water;
    public AudioSource as_buttonDown,as_buttonUp,as_activateObject;
    public AudioSource as_eat1,as_eat2,as_maxSize,as_gamingFish;

    public static bool playing;
    public static int score;
    public static int eaten_gamingfish;
    public static int clearRatio;
    public static string result;//クリアしたか失敗したかなどを保持
    public static SaveData saveData;
    static int timer;
    void Start()
    {
        playing=false;
        as_GlassSnow.Stop();
        GameObject.Find("Time").GetComponent<Text>().text="";
        GameObject.Find("Slider").GetComponent<Slider>().maxValue=Config.Title.delayPlay;

        //セーブデータの読み込みまたは初期化
        if (File.Exists(Config.savePath)){
            string json = File.ReadAllText(Config.savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            saveData=data;
        }else{
            saveData=new SaveData();
            saveData.reset();
        }

        //テキストUIのベストスコアを書き換える
        var scoreText=GameObject.Find("Bestscore").GetComponent<Text>();
        scoreText.text="Best score\n"+saveData.bestScore;
        scoreText.color=saveData.bestColor;

        //リザルト画面からRキーでリトライしていた場合
        if(result!=null)playGame();
    }
    public void playGame(){
        GameObject.Find("Beginning").SetActive(false);
        GameObject.Find("Title").SetActive(false);
        var bestText=GameObject.Find("Bestscore");
        if(bestText!=null)bestText.GetComponent<Text>().text="";
        var slider=GameObject.Find("Slider");
        if(slider!=null)slider.SetActive(false);

        as_GlassSnow.Play();
        result="none";
        clearRatio=0;
        score=Config.initialTimeBonus;
        eaten_gamingfish=0;
        timer = 0;
        playing=true;
        
        //最初の敵を召喚
        for(int i = 0; i < Config.Enemy.initialNum; i++)
        {
            Instantiate(
                enemy_prefab, 
                new Vector3(-6,-20,0), 
                Quaternion.identity, 
                GameObject.Find("General").transform
            );
        }
    }
    void FixedUpdate()
    {
        //タイトル画面の場合はほぼ無視
        if(!playing){
            if (Input.GetKey(KeyCode.Space)){
                playGame();
            }
            return;
        }

        score--;
        if(score<0)score=0;

        //定期的に敵を召喚
        timer=(timer+1)%Config.Enemy.interval;
        if (timer == 0)
        {
            Instantiate(
                enemy_prefab, 
                new Vector3(Config.width*1.5f, Config.height*1.5f, 0),
                Quaternion.identity, 
                GameObject.Find("General").transform
            );
        }

        float max=Config.Player.maxScale;
        float min=Config.Player.initialScale;
        float per=100*(Player.scale.y-min)/(max-min);
        int p=(int)per;
        if(p>100)p=100;
        GameObject.Find("Time").GetComponent<Text>().text=p+"%";
        clearRatio=p;
    }

    //クリア・失敗時にスローモーションにするためのメソッド
    void slowMotion(){
        as_GlassSnow.mute=true;
        as_water.mute=true;
        Time.timeScale=0.02f;
    }

    IEnumerator clear(){
        as_maxSize.PlayOneShot(as_maxSize.clip);
        result = "clear";
        slowMotion();

        yield return new WaitForSeconds(0.012f);
            Time.timeScale=1;
            SceneManager.LoadScene("ResultScene");
    }
    IEnumerator miss()
    {
        result="gameover";
        slowMotion();

        yield return new WaitForSeconds(0.01f);
            Time.timeScale=1;
            SceneManager.LoadScene("ResultScene");
    }
}

//セーブデータのクラス
[System.Serializable]
public class SaveData{
    public int bestScore;
    public Color bestColor;
    public bool red,green,blue,yellow,cian,magenta;
    public void reset(){
        bestScore=0;
        bestColor=new Color(0,0,0);
        red=false;green=false;blue=false;yellow=false;cian=false;magenta=false;
    }
}