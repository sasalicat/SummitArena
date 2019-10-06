using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
public class RemoteClient
{
    public const int BUFFER_SIZE = 1024;
    byte[] buffer;
    Socket socket;
    public RemoteClient(Socket client)
    {
        buffer = new byte[BUFFER_SIZE];
        socket = client;
    }
    public void doit()
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
public class GameEndPoint : MonoBehaviour
{
    public int gamePort = 14747;
    public Socket listenSocket;
    public Thread listenThread;
    public bool BeServer = false;
    protected List<RemoteClient> connects = new List<RemoteClient>();
    protected List<Thread> threads = new List<Thread>();
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
                RemoteClient handleClient = new RemoteClient(client);
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
    public void TryConnect(string ip,int port,int id,string password)
    {
        
        IPAddress sip = IPAddress.Parse(ip);
        IPEndPoint tragetEP = new IPEndPoint(sip, port);
        IPEndPoint localEP = new IPEndPoint(IPAddress.Any, gamePort);
        textShow.Log("gameEndPoint 嘗試linked:" + localEP + "到服務器:" + tragetEP);
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        serverSocket.Bind(localEP);
        serverSocket.Connect(tragetEP);
        textShow.Log("gameEndPoint 連接服務器成功");
        serverSocket.Send(Encoding.UTF8.GetBytes("1~"+id+","+password+"|"));
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
