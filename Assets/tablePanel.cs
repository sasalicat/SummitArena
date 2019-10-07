using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tablePanel : MonoBehaviour {
    public GameObject tableBar;
    public GameObject panel;
    public List<GameObject> objectList = new List<GameObject>();
    public void Init(List<tableData> tlist)
    {
        gameObject.SetActive(true);
        foreach(tableData data in tlist)
        {
            GameObject obj = Instantiate(tableBar, panel.transform);
            obj.GetComponent<tableIcon>().init(data);
            objectList.Add(obj);
            
        }
    }
    public void quit()
    {
        for(int i = 0; i < objectList.Count; i++)
        {
            Debug.Log("quit destory " + objectList[i].name);
            Destroy(objectList[i]);
        }
        objectList.Clear();
        gameObject.SetActive(false);
    }
}
