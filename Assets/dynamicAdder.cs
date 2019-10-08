using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dynamicAdder : MonoBehaviour {

    public GameObject Bar;
    public GameObject panel;
    public List<GameObject> objectList = new List<GameObject>();
    public virtual void aftInstObj(GameObject obj,object arg)
    {

    }
    public void Init(List<object> tlist)
    {
        gameObject.SetActive(true);
        foreach (object data in tlist)
        {
            GameObject obj = Instantiate(Bar, panel.transform);
            aftInstObj(obj,data);
            objectList.Add(obj);
        }
    }
    public void quit()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            Destroy(objectList[i]);
        }
        objectList.Clear();
        gameObject.SetActive(false);
    }
}
