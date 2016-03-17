using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

public class RobotArmController
{
    public float Speed = 0.5f;
    public int Timeout = 60000; // Timeout in milliseconds
    public TcpClient tcpclnt;
    public Stream strm;
    public StreamReader strmrdr;

    public RobotArmController()
    {
        try
        {
            tcpclnt = new TcpClient();
            Console.WriteLine("Connecting");
            
            tcpclnt.Connect("127.0.0.1", 9876);

            strm = tcpclnt.GetStream();
            strmrdr =  new StreamReader(strm);
            ReceiveMessage();
        }

        catch(Exception e)
        {
            Console.WriteLine("Error: " + e.StackTrace);
            Console.ReadKey();
        }
    }

    public RobotArmController(string IpAddress, int Port)
    {
        try
        {
            tcpclnt = new TcpClient();
            Console.WriteLine("Connecting");

            tcpclnt.Connect(IpAddress, Port);

            strm = tcpclnt.GetStream();
            strmrdr = new StreamReader(strm);
            ReceiveMessage();
        }

        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.StackTrace);
            Console.ReadKey();
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

    public void Grab()
    {
        SendMessage("Grab");
    }

    public void Drop()
    {
        SendMessage("Drop");
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
        string result = strmrdr.ReadLine();
        Console.WriteLine(result);
    }
}