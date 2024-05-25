using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


//返回未被占用的用户
public class Serv
{
    //监听套接字
    public Socket listenfd;
    //客户端连接
    public Conn[] conns;
    //最大连接数
    public int maxConn = 50;

    //获取连接，返回-1表示连接失败
    public int NewIndex()
    {
        if (conns == null)
            return -1;
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
            {
                conns[i] = new Conn();
                return i;
            }
            else if (conns[i].isUse == false)
            {
                return i;
            }
        }
        return -1;
    }

    //开启服务器
    public void Start(string host, int port)
    {
        conns = new Conn[maxConn];
        for (int i = 0; i < maxConn; i++)
        {
            conns[i] = new Conn();
        }
        //建立socket连接
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //绑定IP和端口
        IPAddress ipAdr = IPAddress.Parse(host);
        IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
        listenfd.Bind(ipEp);
        //监听连接
        listenfd.Listen(maxConn);
        //继续接受连接
        listenfd.BeginAccept(AcceptCb, null);
        Console.WriteLine("服务器启动成功");
    }

    //给新的连接分配conn
    //接受客户端数据
    //再次调用BeginAccept
    private void AcceptCb(IAsyncResult ar)
    {
        //ar是异步操作对象

        try
        {
            Socket socket = listenfd.EndAccept(ar);
            int index = NewIndex();

            if(index < 0)
            {
                socket.Close();
                Console.WriteLine("警告！！连接已满");
            }
            else
            {
                Conn conn = conns[index];
                conn.Init(socket);
                string adr = conn.GetAdress();
                Console.WriteLine("客户端连接" + adr + "conn池ID: " + index);
                conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
            }
            listenfd.BeginAccept(AcceptCb, null);
        }
        catch (Exception e)
        {
            Console.WriteLine("AcceptCb 失败：" + e.Message);
        }
    }


    //接收并转发消息
    //收到count=0的信号，断开连接
    //继续接收数据
    private void ReceiveCb(IAsyncResult ar)
    {
        Conn conn=(Conn)ar.AsyncState;
        try
        {
            int count = conn.socket.EndReceive(ar);
            //关闭信号
            if(count <= 0)
            {
                Console.WriteLine("收到" + conn.GetAdress() + "断开连接");
                conn.Close();
                return;
            }

            //数据处理
            string str = System.Text.Encoding.UTF8.GetString(conn.readBuff, 0, count);
            Console.WriteLine("收到" + conn.GetAdress() + "数据" + str);
            str = conn.GetAdress() + ": " + str;
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            //广播
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isUse)
                    continue;
                Console.WriteLine("将消息转播给 " + conns[i].GetAdress());
                conns[i].socket.Send(bytes);
            }
            //继续接收
            conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
            
        }
        catch(Exception ex)
        {

            Console.WriteLine("收到" + conn.GetAdress() + "断开连接" + " ex: "+ex);
            conn.Close();
        }
        
    }

class MainClass
    {
        public static void Main(string[] args)
        {
            Serv serv = new Serv();
            serv.Start("127.0.0.1", 1234);
            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "quit":
                        return;
                }
            }
        }
    }
    
}
