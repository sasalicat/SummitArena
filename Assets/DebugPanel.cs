using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DebugPanel : MonoBehaviour,orderHandle {
    public delegate void withDictionary(Dictionary<string,object> args);
    protected List<order> orders = new List<order>();
    protected withDictionary[] functionList=new withDictionary[10];
    public tablePanel tpanel;
    public room rpanel;
    public void handle(order order)
    {
        orders.Add(order);
    }
    public void action0_GetTableList(Dictionary<string, object> args)
    {
        List<tableData> tables = (List<tableData>)args["tables"];
        foreach(tableData data in tables)
        {
            //data
            string msg = "table" + data.index + ">name:" + data.name + " member:";
            foreach(string name in data.memberNames)
            {
                msg += name;
            }
            textShow.Log(msg);
        }
        tpanel.Init(tables);
    } 
    public void action3_CreateTableRequest(Dictionary<string,object> args)
    {
        List<string> list = (List<string>)args["roles"];
        List<object> objList = new List<object>();
        foreach (string str in list) {
            objList.Add(str);
        }
        rpanel.Init(objList);
    }public void action4_AddNewPlayer(Dictionary<string,object> args)
    {
        string player = (string)args["player"];
        rpanel.addNewObj(player);
    }
    public void action5_ChatMessage(Dictionary<string, object> args) {
        rpanel.chat((int)args["index"], (string)args["message"]);
    }
    // Use this for initialization
    void Start () {
        functionList[0]=action0_GetTableList;
        functionList[3] = action3_CreateTableRequest;
        functionList[4] = action4_AddNewPlayer;
        functionList[5] = action5_ChatMessage;
	}
	
	// Update is called once per frame
	void Update () {
        while (orders.Count>0)
        {
            int code = orders[0].code;
            functionList[code](orders[0].args);
            orders.RemoveAt(0);
        }
	}
}
