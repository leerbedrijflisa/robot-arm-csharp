using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

public class RobotArmController
{
    public float Speed = 0.5f;
    public int Timeout = 1; // Timeout in seconds
    private TcpClient tcpclnt;
    private Stream strm;
    private StreamReader strmrdr;

    public RobotArmController()
    {
        try
        {
            tcpclnt = new TcpClient();
            Console.WriteLine("Connecting");
            
            tcpclnt.Connect("127.0.0.1", 9876);

            strm = tcpclnt.GetStream();
            strmrdr =  new StreamReader(strm);
            string response = ReceiveMessage();

            CheckResponse(response, "hello", new string[] { "hello", "bye" });
        }

        catch(SocketException e)
        {
            Console.WriteLine("Kan geen verbinding maken met de server.");
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
            string response = ReceiveMessage();

            CheckResponse(response, "hello", new string[] { "hello", "bye" });
        }

        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.StackTrace);
            Console.ReadKey();
            return;
        }
    }

    public void MoveLeft()
    {
        string response = SendMessage("Move left");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    public void MoveRight()
    {
        string response = SendMessage("Move right");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    public void Grab()
    {
        string response = SendMessage("Grab");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    public void Drop()
    {
        string response = SendMessage("Drop");

        CheckResponse(response, "ok", new string[] { "ok", "bye" });
    }

    private string CheckResponse(string response, string expected, string[] allowed)
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

        throw new ProtocolException("De server heeft geen " + expected + " geantwoord.");
    }

    private string SendMessage(string cmd)
    {
        try
        {
            cmd = cmd + "\r\n";

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(cmd);

            strm.Write(ba, 0, ba.Length);
            return ReceiveMessage();
        }

        catch (Exception e)
        {
            throw new SocketException();
            
        }
    }

    private string ReceiveMessage()
    {
        try
        {
            //Sets the Timeout
            strmrdr.BaseStream.ReadTimeout = Timeout * 1000;
            string result = strmrdr.ReadLine();
            Console.WriteLine(result);
            return result;
        }
        catch (Exception e)
        {
            throw new TimeoutException("Te lang geen antwoord gekregen van de server.");
        }
    }
}