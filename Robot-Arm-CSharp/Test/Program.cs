using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace robotArm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welk programma wilt u uitvoeren? (1 of 2, alleen het nummer)");
            var program = Console.ReadLine();
            switch(program)
            {
                case "1":
                    ProgramOne();
                    break;
                case "2":
                    var task = ProgramTwo();
                    Task.WaitAll(task);
                    break;

                default:
                    Console.WriteLine("Verkeerde waarde ingevoerd.");
                    break;
            }
        }

        static void ProgramOne()
        {
            RobotArmController robotArm;
            using (robotArm = new RobotArmController("127.0.0.1", 9876))
            {
                robotArm.Speed = 0.5f;
                robotArm.MoveLeft();
                robotArm.Speed = 0.9f;
                robotArm.MoveLeft();
                robotArm.Grab();
                Console.Write(robotArm.Scan());
                robotArm.Dispose();
            }
            robotArm.MoveLeft();
        }

        static async Task ProgramTwo()
        {
            AsyncRobotArmController robotArm = new AsyncRobotArmController();
            await robotArm.MoveRight();
            await robotArm.SetSpeed(0.9f);
            await robotArm.MoveLeft();
        }
    }
}
