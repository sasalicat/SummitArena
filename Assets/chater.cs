using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chater : MonoBehaviour {
    public Text text;
    public GameEndPoint ep;
    //public delegate void withString(string s);
    //public withString function;
    public void onClick()
    {
        if (text.text!=null&&text.text != "")
        {
            Debug.Log("requst:" + text.text);
            ep.requst_sendMsg2Room(text.text);
        }
    }
}
