using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tableCreater : MonoBehaviour {
    public Text input;
    public string defaultName;
    public void onClick()
    {
        string name = input.text;
        if(name == null || name == "")
        {
            name = defaultName;
        }
        ConnectClient.main.requst_createTable(name);
        gameObject.SetActive(false);
    }
}
