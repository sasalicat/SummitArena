using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tablePanel : MonoBehaviour {
    public GameObject tableBar;
    public GameObject panel;
    List<GameObject> objectList;
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
        gameObject.SetActive(false);
    }
}
