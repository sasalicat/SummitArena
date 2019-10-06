using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct tableData
{
    public int index;
    public string name;
    public List<string> memberNames;
}
public class order
{
    public int code;
    public Dictionary<string, object> args;
    public order(int code,Dictionary<string,object> args)
    {
        this.code = code;
        this.args = args;
    }
}
public interface orderHandle  {
    void handle(order order);
}
