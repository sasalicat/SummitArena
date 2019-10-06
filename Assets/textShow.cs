using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textShow : MonoBehaviour {
    public RectTransform rect;
    public Text txt;
    protected string inf="";
    public static textShow main;
    private string storeString = "";
    private float initHeight=1220;
    public string information
    {
        set
        {
            inf = value;
            float height = initHeight + txt.fontSize + txt.lineSpacing * 3;
            foreach(char c in inf)
            {
                if(c == '\n')
                {
                    height += txt.fontSize +txt.lineSpacing * 2;
                }
            }
            //Rect sizes = new Rect(rect.rect.x,rect.rect.y,rect.rect.width,height);
            rect.sizeDelta = new Vector2(0, height);
            txt.text = inf;
        }
        get
        {
            return inf;
        }
    }
    void OnEnable()
    {
        //Debug.Log("onEnable");
        if(main == null)
        {
            main = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        //inf = "おはよう世界\nGood Morning World!\n何れ程　歩いたろう？\n足の痛みだけが\nその距離を物語る\n長い夜を越えた絶景への期待が\n今日　僕を生かしている\n神々の霊峰　新緑の宮殿\n岩窟の最奥　蒼穹の涯\n踏破してみせる　限界は無い\n惑星(ほし)の隅々まで\nおはよう世界\nGood Morning World！\n不可能の闇を祓って\n神話を日常に変えていく Odyssey\n苔生す意志に導かれ\n世界を拡げよう\n何れ程　間違っても\n胸の鼓動だけは\nこの生命を讃える\n幾億　幾兆の分岐が紡ぎ出す\n唯一つの「今」を\nWho am I？\n迷い　駆けていく\n不安の雨に打たれても\n救世の英雄　博愛の賢者\n伝統の後継　革命の旗手\n何だってなれる　定形は無い\n人体(ヒト)の半分は水\n";
        inf = "hello world";
        information = inf;
// initHeight = rect.rect.height;
    }
    void Update()
    {
        information += storeString;
        storeString = "";
    }
    public static void Log(string msg)
    {
        msg += '\n';
        main.storeString += msg;
    }
}
