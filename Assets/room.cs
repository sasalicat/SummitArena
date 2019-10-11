using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room : dynamicAdder {
    public delegate void withPlayerList(List<string> list);

    public static room Main;
    public withPlayerList afterRoomChange;
    public textPoster chatBar;
    protected List<string> playerNames=new List<string>();
    
    public void OnEnable()
    {
        if (Main != null)
        {
            Destroy(this);
        }
        else
        {
            Main = this;
        }
    }
    public override void aftInstObj(GameObject obj, object arg)
    {
        obj.GetComponent<playerBar>().init((string)arg);
        playerNames.Add((string)arg);
    }
    public virtual void addNewObj(object arg)
    {
        GameObject obj = Instantiate(Bar, panel.transform);
        objectList.Add(obj);
        obj.GetComponent<playerBar>().init((string)arg);
        playerNames.Add((string)arg);
        if(afterRoomChange!=null)
            afterRoomChange(playerNames);
    }
    public void chat(int pidx, string msg) {
        chatBar.store(playerNames[pidx] + ":" + msg);
    }
}
