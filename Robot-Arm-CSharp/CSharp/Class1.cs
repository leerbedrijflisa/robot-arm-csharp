using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

public class CSharp
{
    public float Speed = 0.5f;
    public int Timeout = 60000; // Timeout in milliseconds
    public TcpClient tcpclnt;
    public Stream strm;

    public CSharp()
    {
        try
        {
            tcpclnt = new TcpClient();
            Console.WriteLine("Connecting");
            
            tcpclnt.Connect("127.0.0.1", 9876);

            strm = tcpclnt.GetStream();
            ReceiveMessage();
        }

        catch(Exception e)
        {
            Console.WriteLine("Error: " + e.StackTrace);
        }
    }

    public void MoveLeft()
    {
        SendMessage("Move left");
    }

    public void MoveRight()
    {
        SendMessage("Move right");
    }

    public void SendMessage(string cmd)
    {
        try
        {
            cmd = cmd + "\r\n";

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(cmd);

            strm.Write(ba, 0, ba.Length);
            ReceiveMessage();
                
        }

        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.StackTrace);
        }
    }

    public void ReceiveMessage()
    {
        byte[] bb = new byte[100];
        int k = strm.Read(bb, 0, 100);
        string result = "";
        for (int i = 0; i < k; i++)
        {
            Console.Write(Convert.ToChar(bb[i]).ToString());
        }
        Console.WriteLine();
    }
}