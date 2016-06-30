using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public class RobotArmController : IDisposable
{
    bool disposed = false;
    SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
    private float robotArmSpeed;
    public float robotArmTimeout = 60; // Timeout in seconds
    private TcpClient tcpclnt;
    private Stream strm;
    private StreamReader strmrdr;

    public void Dispose()
    {
        Dispose(true);
        strm.Close();
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            handle.Dispose();
        }

        disposed = true;
    }

    public float Timeout
    {
        get
        {
            return robotArmTimeout;
        }
        set
        {
            robotArmTimeout = value;
            strmrdr.BaseStream.ReadTimeout = (int)value * 1000;
        }
    }


    public float Speed
    {
        get
        {
            return robotArmSpeed;
        }
        set
        {
            robotArmSpeed = (float)Math.Round(value * 10) / 10;
            if(value >= 0.0f && value <= 1.0f)
            {   

                string response = SendMessage("speed " + robotArmSpeed * 100);
                CheckResponse(response, "ok", new string[] { "ok", "bye" });
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    public RobotArmController()
    {
        try
        {
            tcpclnt = new TcpClient();
            
            tcpclnt.Connect("127.0.0.1", 9876);

            strm = tcpclnt.GetStream();
            strmrdr =  new StreamReader(strm);
            string response = ReceiveMessage();
            
            CheckResponse(response, "hello", new string[] { "hello", "bye" });
        }

        catch(Exception e)
        {
             throw new SocketException();
        }
    }

    public RobotArmController(string IpAddress, int Port)
    {
        try
        {
            tcpclnt = new TcpClient();

            tcpclnt.Connect(IpAddress, Port);

            strm = tcpclnt.GetStream();
            strmrdr = new StreamReader(strm);
            string response = ReceiveMessage();

            CheckResponse(response, "hello", new string[] { "hello", "bye" });
        }

        catch (Exception e)
        {
            throw new SocketException();
        }
    }

    public void MoveLeft()
    {
        CheckStream();
        string response = SendMessage("Move left");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    public void MoveRight()
    {
        CheckStream();
        string response = SendMessage("Move right");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    public void Grab()
    {
        CheckStream();
        string response = SendMessage("Grab");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    public void Drop()
    {
        CheckStream();
        string response = SendMessage("Drop");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    public Color Scan()
    {
        CheckStream();
        string response = SendMessage("Scan");

        CheckResponse(response, "A colour", new string[] { "red", "green", "blue", "white", "none", "bye" });
        Color color;
        if (response == Color.Red.ToString().ToLower())
        {
            color = Color.Red;
            return color;
        }
        else if (response == Color.Blue.ToString().ToLower())
        {
            color = Color.Blue;
            return color;
        }
        else if (response == Color.Green.ToString().ToLower())
        {
            color = Color.Green;
            return color;
        }
        else if (response == Color.White.ToString().ToLower())
        {
            color = Color.White;
            return color;
        }
        else
        {
            color = Color.None;
            return color;
        }
    }

    public void LoadLevel(string level)
    {
        CheckStream();
        string response = SendMessage("load " + level);

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    private void CheckStream()
    {
        if (strm == null)
        {
            throw new SocketException();
        }
    }

    private void CheckResponse(string response, string expected, string[] allowed)
    {
        var correctResponse = false;
        response = response.Replace("\n", "");

        for (int i = 0; i < allowed.Length; i++)
        {
            if(allowed[i] == response)
            {
                correctResponse = true;
                break;
            }
        }
        if(!correctResponse)
        {
            throw new ProtocolException("The server didn't return " + expected + ".");
        }
    }

    private string SendMessage(string cmd)
    {
        CheckStream();
        cmd = cmd + "\r\n";

        ASCIIEncoding asen = new ASCIIEncoding();
        byte[] ba = asen.GetBytes(cmd);

        strm.Write(ba, 0, ba.Length);
        return ReceiveMessage();
    }

    private string ReceiveMessage()
    {
        try
        {
            CheckStream();
            string result = strmrdr.ReadLine();
            if (result == "bye")
            {
                throw new SocketException();
            }
            return result;
        }
        catch (Exception e)
        {
            throw new TimeoutException();
        }
    }
}