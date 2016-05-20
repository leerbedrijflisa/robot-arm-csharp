using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public class AsyncRobotArmController : IDisposable
{
    bool disposed = false;
    SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
    private float robotArmSpeed;
    public float robotArmTimeout = 120; // Timeout in seconds
    private TcpClient tcpclnt;
    private Stream strm;
    private StreamReader strmrdr;
    private bool initialized = false;
    private string ipaddress = "127.0.0.1";
    private int port = 9876;

    public void Dispose()
    {
        Dispose(true);
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

    public AsyncRobotArmController()
    {
        
    }

    public AsyncRobotArmController(string IpAddress, int Port)
    {
        ipaddress = IpAddress;
        port = Port;
    }

    async private Task Initialize()
    {
        try
        {
            tcpclnt = new TcpClient();

            tcpclnt.Connect(ipaddress, port);

            strm = tcpclnt.GetStream();
            strmrdr = new StreamReader(strm);
            string response = await ReceiveMessage();

            CheckResponse(response, "hello", new string[] { "hello", "bye" });
            initialized = true;
        }

        catch (Exception e)
        {
            throw new SocketException();
        }
    }

    async public Task MoveLeft()
    {
        await CheckInit();
        CheckStream();
        string response = await SendMessage("Move left");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    async public Task MoveRight()
    {
        await CheckInit();
        CheckStream();
        string response = await SendMessage("Move right");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    async public Task Grab()
    {
        await CheckInit();
        CheckStream();
        string response = await SendMessage("Grab");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    async public Task Drop()
    {
        await CheckInit();
        CheckStream();
        string response = await SendMessage("Drop");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    async public Task<Color> Scan()
    {
        await CheckInit();
        CheckStream();
        string response = await SendMessage("Scan");

        CheckResponse(response, "A colour", new string[] { "red", "green", "blue", "white", "none", "bye" });
        Color color;
        if (response == Color.Red.ToString())
        {
            color = Color.Red;
            return color;
        }
        else if (response == Color.Blue.ToString())
        {
            color = Color.Blue;
            return color;
        }
        else if (response == Color.Green.ToString())
        {
            color = Color.Green;
            return color;
        }
        else if (response == Color.White.ToString())
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

    async public Task SetSpeed(float speed)
    {
        await CheckInit();
        CheckStream();
        if (speed >= 0.0f && speed <= 1.0f)
        {
            robotArmSpeed = (float)Math.Round(speed * 10) / 10;
            robotArmSpeed = speed * 100;
            string response = await SendMessage("speed " + robotArmSpeed);
            CheckResponse(response, "ok", new string[] { "ok", "bye" });
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    async private Task CheckInit()
    {
        if (initialized == false)
        {
            await Initialize();
        }
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
            if (allowed[i] == response)
            {
                correctResponse = true;
                break;
            }
        }
        if (!correctResponse)
        {
            throw new ProtocolException("The server didn't return " + expected + ".");
        }
    }

    async private Task<string> SendMessage(string cmd)
    {
        CheckStream();
        cmd = cmd + "\r\n";

        ASCIIEncoding asen = new ASCIIEncoding();
        byte[] ba = asen.GetBytes(cmd);

        await strm.WriteAsync(ba, 0, ba.Length);
        return await ReceiveMessage();
    }

    async private Task<string> ReceiveMessage()
    {
        try
        {
            CheckStream();
            string result = await strmrdr.ReadLineAsync();
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