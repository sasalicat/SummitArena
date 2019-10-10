using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerBar : MonoBehaviour {
    public Text roleName;
    public void init(string name)
    {
        roleName.text = name;
    }
}
