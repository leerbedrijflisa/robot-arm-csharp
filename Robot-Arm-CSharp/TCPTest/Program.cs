
using System;
using System.IO;
using System.Text;
using System.Net.Sockets;


public class TCPTest
{

    public static void Main()
    {
        Connect();
    }

    public static void Connect()
    {
        try
        {
            TcpClient tcpclnt = new TcpClient();
            Console.WriteLine("Connecting");
            
            tcpclnt.Connect("127.0.0.1", 9876);

            Stream strm = tcpclnt.GetStream();
            ReceiveMessage(tcpclnt, strm);
        }

        catch(Exception e)
        {
            Console.WriteLine("Error: " + e.StackTrace);
        }
    }

    public static void SendMessage(TcpClient tcpclnt, Stream strm)
    {
        try
        {
            string cmd = Console.ReadLine();
            cmd = cmd + "\r\n";

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(cmd);

            strm.Write(ba, 0, ba.Length);
            ReceiveMessage(tcpclnt, strm);
                
        }

        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.StackTrace);
        }
    }

    public static void ReceiveMessage(TcpClient tcpclnt, Stream strm)
    {
        byte[] bb = new byte[100];
        int k = strm.Read(bb, 0, 100);
        for (int i = 0; i < k; i++)
            Console.Write(Convert.ToChar(bb[i]));
        SendMessage(tcpclnt, strm);
    }
}