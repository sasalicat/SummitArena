using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class ConnectClient : MonoBehaviour {
    public string serverIP= "127.0.0.1";
    public int serverPort = 6000;
    public int selfPort = 9487;
    public string username = "Jhon Cean!";
    protected byte[] buffer;
    public const int BUFFER_SIZE = 4096;
    protected Socket SocketClient;
    protected Thread ThreadClient;
    private int idInTable;
    private string password;
    public GameEndPoint gEndPoint;
    public DebugPanel panel;
    public static ConnectClient main;
    public void OnEnable()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public  void readMsg(string msg)
    {
        string[] orders = msg.Split('|');
        foreach (string order in orders)
        {
            if (order != "")//split會切出""
            {

                string[] part = order.Split('~');//使用~切分action code 和 action arg
                if (part.Length != 2)
                {
                    textShow.Log("order:" + order + "格式錯誤,不予處理");
                    continue;
                }
                int code;
                if (int.TryParse(part[0], out code))
                {
                    switch (code)
                    {
                        case 0:
                            {
                                textShow.Log("table 列表:" + part[1]);
                                Dictionary<string, object> args=new Dictionary<string, object>();
                                List<tableData> list = new List<tableData>();
                                string[] tables = part[1].Split(':');
                                foreach(string table in tables)
                                {
                                    string[] infs = table.Split(';');
                                    tableData data = new tableData();
                                    int idx;

                                    if(!int.TryParse(infs[0],out idx))
                                    {
                                        textShow.Log("table data:" + table + "不符合格式");
                                        break;
                                    }
                                    data.index = idx;
                                    data.name = infs[2];
                                    string inf = infs[3];
                                    string[] names = inf.Split('/');
                                    List<string> nlist = new List<string>(names);
                                    data.memberNames = nlist;
                                    list.Add(data);
                                }
                                args["tables"] = list;
                                order newone = new order(0,args);
                                panel.handle(newone);
                                break;
                            }
                        case 1:
                            {
                                string[] subs = part[1].Split(',');
                                int id;
                                if (!int.TryParse(subs[0], out id))
                                {
                                    textShow.Log("解析greeting request 錯誤的id:" + subs[0] + "不是數字");
                                }
                                else
                                {
                                    idInTable = id;
                                    password = subs[1];
                                    textShow.Log("登入tableServer完成,收到id:" + idInTable + "password:" + password);
                                    gEndPoint.TryConnect(serverIP,serverPort, idInTable, password);
                                }
                                break;
                            }
                        case 2:
                            {
                                string[] subs = part[1].Split(',');
                                string ip = subs[0];
                                int port;

                                if (!int.TryParse(subs[1], out port))
                                {
                                    string packet = "5~3|";
                                    SocketClient.Send(Encoding.UTF8.GetBytes(packet));
                                }
                                else
                                {
                                    Console.WriteLine("收到Connect Requst");
                                    Console.WriteLine("連接請求 ip:" + ip + " port:" + port);

                                    


                                }

                                break;
                            }
                        case 3:
                            {
                                string[] subs = part[1].Split(';');
                                int c;
                                if (!int.TryParse(subs[0], out c))
                                {
                                    textShow.Log("解析greeting request 錯誤的code:" + subs[0] + "不是數字");
                                }
                                else
                                {
                                    if(code == 1)
                                    {
                                        Dictionary<string, object> args = new Dictionary<string, object>();
                                        List<string> names = new List<string>(subs[1].Split('/'));
                                        args["roles"] = names;
                                        panel.handle(new order(3, args));
                                    }
                                    else
                                    {
                                        textShow.Log("創建房間失敗,返回code" + c);
                                    }
                                    
                                }
                                break;
                            }


                    }
                }
                else
                {
                    textShow.Log("order:" + order + " order code不是數字,不予處理");
                    continue;
                }
            }
        }
    }
    private static string receiveMsg(Socket socket)
    {
        byte[] arrRecvmsg = new byte[10 * 1024];
        try
        {
            //定義一個1M的記憶體緩衝區，用於臨時性儲存接收到的訊息  

            //將客戶端套接字接收到的資料存入記憶體緩衝區，並獲取長度  
            int length = ((Socket)socket).Receive(arrRecvmsg);
            if (length == 0)
            {
                textShow.Log("服務器切斷連接");
                return null;
            }
            //將套接字獲取到的字元陣列轉換為人可以看懂的字串  
            return Encoding.UTF8.GetString(arrRecvmsg, 0, length);

        }
        catch (Exception ex)
        {
            textShow.Log("遠端伺服器已經中斷連線！" + ex.Message + "\r\n");
            return null;
        }
    }
    private void greeting()
    {
        string greetingCode = "0~"+username+"|";
        SocketClient.Send(Encoding.UTF8.GetBytes(greetingCode));
        textShow.Log("向tableServer 發送greeting");
        //string msg = receiveMsg(SocketClient);
        //readMsg(msg);
    }
	public void Recv(object socket)
    {
        greeting();
        
        while (true)
        {
            try
            {
                Debug.Log("recv");
                //定義一個1M的記憶體緩衝區，用於臨時性儲存接收到的訊息  

                //將客戶端套接字接收到的資料存入記憶體緩衝區，並獲取長度  
                int length = ((Socket)socket).Receive(buffer);
                if (length == 0)
                {
                    Console.WriteLine("服務器切斷連接");
                    break;
                }
                //將套接字獲取到的字元陣列轉換為人可以看懂的字串  
                string strRevMsg = Encoding.UTF8.GetString(buffer, 0, length);
                
                readMsg(strRevMsg);
            }
            catch (Exception ex)
            {
                textShow.Log("遠端伺服器已經中斷連線！" + ex.Message + "\r\n");
                break;
            }
        }
    }

    public void startConnect() {
        //創建buffer
        buffer = new byte[BUFFER_SIZE];

        //嘗試連接table server
        IPAddress ip = IPAddress.Parse(serverIP);
        IPEndPoint ipe = new IPEndPoint(ip, serverPort);
        IPEndPoint localEP = new IPEndPoint(IPAddress.Any, selfPort);
        SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        SocketClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        SocketClient.Bind(localEP);
        textShow.Log("連接tableServer: " + serverIP + ":" + serverPort);
        try
        {
            //客戶端套接字連線到網路節點上，用的是Connect  
            SocketClient.Connect(ipe);
            textShow.Log("成功與目標tableServer連接");
            gEndPoint.startWaiting();
            //本客戶端的ID
            //Random rand = new Random();
            //string sendsStr = "num"+rand.Next(0,10);
            // ClientSendMsg(sendsStr);
        }
        catch (Exception e)
        {
            textShow.Log("連接失敗,Exception:" + e);
            return;
        }
        ThreadClient = new Thread(Recv);
        ThreadClient.IsBackground = true;
        ThreadClient.Start(SocketClient);
    }
    void Start()
    {
        Debug.Log("connect Start");
        startConnect();
    }
    void OnDestroy()
    {
        Debug.Log("on destory");
        //SocketClient.Shutdown(SocketShutdown.Both);
        if (ThreadClient != null)
        {
            ThreadClient.Abort();
            ThreadClient = null;
        }
        if (SocketClient != null)
        {
            Debug.Log("socket close");
            SocketClient.Shutdown(SocketShutdown.Both);
            SocketClient.Close();
        }

    }
    public void requst_requstTableList()
    {
        string packet = "2~0|";
        SocketClient.Send(Encoding.UTF8.GetBytes(packet));
    }
    public void requst_createTable(string tablename)
    {
        string packet = "3~" + tablename + '|';
        SocketClient.Send(Encoding.UTF8.GetBytes(packet));
        gEndPoint.BeServer = true;
    }
}
