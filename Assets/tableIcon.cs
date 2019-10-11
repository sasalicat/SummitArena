using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tableIcon : MonoBehaviour {
    public Text txt;
    public int index;
    public delegate void withNone();
    public withNone afterClick;
    public void init(tableData data)
    {
        index = data.index;
        string msg = data.name + ": ";
        foreach(string name in data.memberNames)
        {
            msg += name + ",";
        }
        txt.text = msg;
    }
    public void init(tableData data,withNone callback)
    {
        init(data);
        afterClick += callback;
    }
    public void onClick()
    {
        ConnectClient.main.requst_connectTable(index);
        if (afterClick != null)
            afterClick();
    }
    
}
