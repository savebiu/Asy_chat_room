using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class net : MonoBehaviour
{
    //服务器的IP和端口
    public InputField hostinput;
    public InputField portinput;
    
    //显示客户端收到的消息
    public Text recvText;
    public string recvStr;

    //显示客户端ip和端口
    public Text clientText;
    //聊天输入框
    public InputField textInput;
    
    //socket和接收缓冲区
    Socket socket;
    const int BUFFER_SIZE = 1024;
    public byte[] readBuff = new byte[BUFFER_SIZE];
    
    //连接Connetion按钮按下socket-connect连接服务器，再调用BeginReceive异步接收
    public void Connetion()
    {
        recvText.text = "";
        //建立socket
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        //连接
        string host = hostinput.text;
        int port = int.Parse(portinput.text);
        socket.Connect(host, port);
        clientText.text = "客户端地址" + socket.LocalEndPoint.ToString();
        //接收
        socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
    }

    //异步接收回调ReciverCb,先解析服务端消息，将聊天语句存入recvStr,再调用BeginRecive开启下一次异步接收
    public void ReceiveCb(IAsyncResult ar)

    {
        try
        {
            int count = socket.EndReceive(ar);
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            if (recvStr.Length > 300)
                recvStr = "接收的消息：";
            recvStr += str + "\n";
            socket.BeginReceive(readBuff, 0 ,BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);

        }
        catch (Exception e)
        {
            recvText.text += "连接断开";
            socket.Close();
        }
        
    }

    //Send将textInput的内容发送给服务端
    public void Send()
    {
        string str = textInput.text;
        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        try
        {
            socket.Send(bytes);
        }
        catch { }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        recvText.text = recvStr;
    }
}
