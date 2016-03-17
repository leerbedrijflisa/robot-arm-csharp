using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var RobotArm = new RobotArmController();
            RobotArm.MoveLeft();
            RobotArm.Grab();
            RobotArm.MoveRight();
            RobotArm.Drop();
            RobotArm.Speed = 100;
            Console.ReadKey();
        }
    }
}
