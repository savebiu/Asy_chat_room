using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class net : MonoBehaviour
{
    //��������IP�Ͷ˿�
    public InputField hostinput;
    public InputField portinput;
    
    //��ʾ�ͻ����յ�����Ϣ
    public Text recvText;
    public string recvStr;

    //��ʾ�ͻ���ip�Ͷ˿�
    public Text clientText;
    //���������
    public InputField textInput;
    
    //socket�ͽ��ջ�����
    Socket socket;
    const int BUFFER_SIZE = 1024;
    public byte[] readBuff = new byte[BUFFER_SIZE];
    
    //����Connetion��ť����socket-connect���ӷ��������ٵ���BeginReceive�첽����
    public void Connetion()
    {
        recvText.text = "";
        //����socket
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        //����
        string host = hostinput.text;
        int port = int.Parse(portinput.text);
        socket.Connect(host, port);
        clientText.text = "�ͻ��˵�ַ" + socket.LocalEndPoint.ToString();
        //����
        socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
    }

    //�첽���ջص�ReciverCb,�Ƚ����������Ϣ��������������recvStr,�ٵ���BeginRecive������һ���첽����
    public void ReceiveCb(IAsyncResult ar)

    {
        try
        {
            int count = socket.EndReceive(ar);
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            if (recvStr.Length > 300)
                recvStr = "���յ���Ϣ��";
            recvStr += str + "\n";
            socket.BeginReceive(readBuff, 0 ,BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);

        }
        catch (Exception e)
        {
            recvText.text += "���ӶϿ�";
            socket.Close();
        }
        
    }

    //Send��textInput�����ݷ��͸������
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
