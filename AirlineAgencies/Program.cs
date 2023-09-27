using AirlineAgencies.EntityFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAgencies
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Please input valid parameters.");
                return;
            }

            if (!DateTime.TryParse(args[0], out DateTime startDate) ||
                !DateTime.TryParse(args[1], out DateTime endDate) ||
                !int.TryParse(args[2], out int agencyId))
            {
                Console.WriteLine("Invalid input parameters.");
                return;
            }

            Console.WriteLine("Start : Airline system change algorithm.");

            Functionality AirlinesAlgorithm = new Functionality();
            AirlinesAlgorithm.ChangeDetectionAlgorithm(startDate, endDate, agencyId);

            Console.WriteLine("End : Airline system change algorithm.");

            Console.WriteLine("Press anykey to exit.");
            Console.ReadKey();
        }
    }
}
