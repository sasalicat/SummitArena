using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DebugPanel : MonoBehaviour,orderHandle {
    public delegate void withDictionary(Dictionary<string,object> args);
    protected List<order> orders = new List<order>();
    protected List<withDictionary> functionList=new List<withDictionary>();
    public tablePanel tpanel;
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

    // Use this for initialization
    void Start () {
        functionList.Add(action0_GetTableList);
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
