﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
public class RemoteEndPoint
{
    public const int BUFFER_SIZE = 1024;
    byte[] buffer;
    protected Socket socket;
    protected GameEndPoint local_ep;
    protected virtual void decodeMsg(string msg) {
    }
    public RemoteEndPoint(Socket client,GameEndPoint local_ep)
    {
        buffer = new byte[BUFFER_SIZE];
        socket = client;
        this.local_ep = local_ep;
    }
    public virtual void doit()
    {
        while (true)
        {
            try
            {
                //定義一個1M的記憶體緩衝區，用於臨時性儲存接收到的訊息  

                //將客戶端套接字接收到的資料存入記憶體緩衝區，並獲取長度  
                int length = ((Socket)socket).Receive(buffer);
                if (length == 0)
                {
                    textShow.Log("遠端client切斷連接");
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    break;
                }
                //將套接字獲取到的字元陣列轉換為人可以看懂的字串  
                string strRevMsg = Encoding.UTF8.GetString(buffer, 0, length);
                textShow.Log("收到信息:" + strRevMsg);
                decodeMsg(strRevMsg);
            }
            catch (Exception ex)
            {
                textShow.Log("client已經中斷連線！" + ex.Message + "\r\n");
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                break;
            }
        }
    }
    public void Close()
    {
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}
public class RemoteClient: RemoteEndPoint
{
    public RemoteClient(Socket s,GameEndPoint ep):base(s,ep)
    {
       
    }
    protected virtual void requstEnterRoom()
    {
        string packet = "0~";

        for (int i = 0;i<room.Main.playerNames.Count;i++)
        {
            if (i == 0)
            {
                packet += room.Main.playerNames[0];
            }
            else
            {
                packet += "/"+room.Main.playerNames[i];
            }

        }
        packet += "|";
        socket.Send(Encoding.UTF8.GetBytes(packet));
    }
    protected override void decodeMsg(string msg)
    {
        string[] orders = msg.Split('|');
        foreach (string order in orders)
        {
            if (order != "")//split會切出""
            {
                string[] parts = msg.Split('~');
                int code;
                if (int.TryParse(parts[0], out code))
                {
                    switch (code)
                    {
                        case 0://client say hello 
                            {
                                Dictionary<string, object> args = new Dictionary<string, object>() { { "player", parts[1] } };
                                local_ep.handler.handle(new order(4, args));
                                requstEnterRoom();//把當前房間里有誰告訴client
                                break;
                            }
                    }
                }
                else
                {
                    textShow.Log("來自RemoteClient的錯誤format:" + msg);
                }
            }
        }

    }

}
public class RemoteServer : RemoteEndPoint
{
    
    public RemoteServer(Socket s,GameEndPoint ep) : base(s,ep)
    {

    }
    public override void doit()
    {
        string name = ConnectClient.main.username;
        socket.Send(Encoding.UTF8.GetBytes("0~" + name + "|"));//發送greeting code
        base.doit();
    }
}
public class GameEndPoint : MonoBehaviour
{
    public int gamePort = 14747;
    public Socket listenSocket;
    public Thread listenThread;
    public bool BeServer = false;
    public DebugPanel handler;
    protected List<RemoteClient> connects = new List<RemoteClient>();
    protected List<Thread> threads = new List<Thread>();
    protected RemoteServer server;
    protected Thread sthread;
    // Use this for initialization

    void listenFunction(object socket)
    {
        while (true)
        {
            Socket client = ((Socket)socket).Accept();
            textShow.Log("gameClient接收到連接");
            if (BeServer)
            {
                textShow.Log("處理新的client");
                RemoteClient handleClient = new RemoteClient(client,this);
                Thread forSelfClient = new Thread(handleClient.doit);
                threads.Add(forSelfClient);
                connects.Add(handleClient);
                forSelfClient.IsBackground = false;
                forSelfClient.Start();

            }
            else
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
        }
    }
    void tempReceive(object socket) {
        byte[] temp = new byte[1024];
        ((Socket)socket).Receive(temp);
        ((Socket)socket).Shutdown(SocketShutdown.Both);
        ((Socket)socket).Close();
    }
    public void startWaiting()
    {
        textShow.Log("gameEndPoint 開始等待連接");
        listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        IPEndPoint serverIPE = new IPEndPoint(IPAddress.Any, gamePort);
        listenSocket.Bind(serverIPE);
        listenSocket.Listen(20);
        listenThread = new Thread(listenFunction);
        listenThread.IsBackground = true;
        listenThread.Start(listenSocket);
        textShow.Log("gameEndPoint 成功打開listenSocket");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void startConnect(string ip, int port, int id, string password)
    {
        try {
            IPAddress sip = IPAddress.Parse(ip);
            IPEndPoint tragetEP = new IPEndPoint(sip, port);
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, gamePort);
            textShow.Log("gameEndPoint 嘗試linked:" + localEP + "到服務器:" + tragetEP);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            serverSocket.Bind(localEP);
            serverSocket.Connect(tragetEP);
            textShow.Log("gameEndPoint 連接服務器成功");
            serverSocket.Send(Encoding.UTF8.GetBytes("1~" + id + "," + password + "|"));
        }
        catch (Exception e)
        {
            textShow.Log("gameEndPoint連接服務器失敗錯誤信息:\n" + e);
        }
    }
    public void conncetRemoteEndPoint(string ip,int port)
    {
        try
        {


            IPAddress sip;
            if(!IPAddress.TryParse(ip,out sip))
            {
                ConnectClient.main.requst_answerConnectRemote(2);
                return;
            }

            IPEndPoint tragetEP = new IPEndPoint(sip, port);
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, gamePort);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            serverSocket.Bind(localEP);
            serverSocket.Connect(tragetEP);
            if (BeServer)
            {
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
                //Thread temp = new Thread(tempReceive);
                //temp.IsBackground = true;
                //temp.Start(serverSocket);
                
            }
            else {
                server = new RemoteServer(serverSocket,this);
                sthread = new Thread(server.doit);
                sthread.IsBackground = true;
                sthread.Start();
                
            }
            ConnectClient.main.requst_answerConnectRemote(1);
        }
        catch (Exception e)
        {
            ConnectClient.main.requst_answerConnectRemote(0);
            textShow.Log("gameEndPoint連接遠端失敗\nip:"+ip+"port:"+port+" 錯誤信息:\n" + e);
        }
    }
    void OnDestroy()
    {
        if(listenThread!=null)
            listenThread.Abort();
        if (listenSocket != null)
        {
            //listenSocket.Shutdown(SocketShutdown.Both);
            listenSocket.Close();
        }
        foreach (Thread thread in threads) {
            thread.Abort();
        }
        foreach(RemoteClient client in connects)
        {
            client.Close();
        }
    }
}
