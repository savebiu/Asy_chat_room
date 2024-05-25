using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


//用户池
public class Conn
{
    public const int BUFFER_SIZE = 1024;
    public Socket socket;
    public bool isUse = false;
    public byte[] readBuff = new byte[BUFFER_SIZE];
    public int buffCount = 0;

    public Conn()
    {
        readBuff = new byte[BUFFER_SIZE];
    }

    //初始化服务器
    public void Init(Socket socket)
    {
        this.socket = socket;
        isUse = true;
        buffCount = 0;
    }

    //缓冲区剩余字节
    public int BuffRemain()
    {
        return BUFFER_SIZE - buffCount;
    }

    //获取客户端地址
    public string GetAdress()
    {
        if (!isUse)
        {
            return "无法获取地址";

        }
        return socket.RemoteEndPoint.ToString();
    }

    //关闭
    public void Close()
    {
        if (!isUse)
            return;
        Console.WriteLine("断开连接" + GetAdress());
        socket.Close();
        isUse = false;
    }
}
