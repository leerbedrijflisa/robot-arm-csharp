using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace robotArm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welk programma wilt u uitvoeren? (1, 2, 3 of 4, alleen het nummer)");
            var program = Console.ReadLine();
            switch(program)
            {
                case "1":
                    ProgramOne();
                    break;
                case "2":
                    ProgramTwo();
                    break;
                case "3":
                    ProgramThree();
                    break;
                case "4":
                    ProgramFour();
                    break;
                default:
                    Console.WriteLine("Verkeerde waarde ingevoerd.");
                    break;
            }
        }

        static void ProgramOne()
        {
            var robotArm = new RobotArmController();
            robotArm.MoveLeft();
            robotArm.Grab();
            robotArm.MoveRight();
            robotArm.Drop();
        }

        static void ProgramTwo()
        {
            var robotArm = new RobotArmController();
            for (int i = 0; i < 4; i++)
            {
                robotArm.MoveLeft();
            }
            robotArm.Grab();
            Color color = robotArm.Scan();
            switch(color)
            {
                case Color.red:
                    robotArm.MoveRight();
                    robotArm.Drop();
                    break;

                case Color.green:
                    for (int i = 0; i < 2; i++)
                    {
                        robotArm.MoveRight();
                    }
                    robotArm.Drop();
                    break;

                case Color.blue:
                    for (int i = 0; i < 3; i++)
                    {
                        robotArm.MoveRight();
                    }
                    robotArm.Drop();
                    break;

                case Color.white:
                    robotArm.Drop();
                    break;

                case Color.none:
                    for (int i = 0; i < 4; i++)
                    {
                        robotArm.MoveRight();
                        robotArm.Drop();
                    }
                    break;

                default:
                    robotArm.Drop();
                    break;
            }
        }

        static void ProgramThree()
        {
            var robotArm = new RobotArmController();
            robotArm.MoveLeft();
            robotArm.Speed = 1.0f;
            robotArm.MoveRight();
            robotArm.Speed = 0.5f;
            robotArm.MoveLeft();
        }

        static void ProgramFour()
        {
            var robotArm = new RobotArmController("127.0.0.1", 9876);
            robotArm.Timeout = 1;
            robotArm.Speed = 0.0f;
            robotArm.MoveLeft();
        }

        static void ProgramFive()
        {
            using (var robotArm = new RobotArmController())
            {
                robotArm.MoveLeft();
                robotArm.Drop();
            }
        }
    }
}
