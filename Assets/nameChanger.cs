using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class nameChanger : MonoBehaviour {
    public Text Input;
    public void onClick()
    {
        if (Input.text != null || Input.text != "")
        {
            ConnectClient.main.username = Input.text;
        }
        textShow.Log("改名為:" + ConnectClient.main.username);
        ConnectClient.main.requst_rename();
    }
    
}
