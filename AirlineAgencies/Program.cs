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
            //2016-10-11 2016-10-13 1
            if (!DateTime.TryParse(args[0], out DateTime startDate) ||
                !DateTime.TryParse(args[1], out DateTime endDate) ||
                !int.TryParse(args[2], out int agencyId))
            {
                Console.WriteLine("Invalid input parameters.");
                return;
            }

            Functionality detector = new Functionality();
            detector.RunChangeDetectionAlgorithm(startDate, endDate, agencyId);

            Console.ReadKey();
        }
    }
}
