using AirlineAgencies.EntityFrameWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAgencies
{
    public class Functionality
    {
        private AirlineAgenciesDbContext dbContext = new AirlineAgenciesDbContext();
        private DateTime currentDate = DateTime.Now;

        public void RunChangeDetectionAlgorithm(DateTime startDate, DateTime endDate, Int64 agencyId)
        {
            DateTime startTime = DateTime.Now;

            #region DetectNewFlights
              var newFlights = CheckNewFlights(startDate, endDate);
            #endregion

            #region DetectDiscontinuedFlights
                var discontinuedFlights = CheckDiscontinuedFlights(startDate, endDate);
            #endregion DetectNewFlights

            WriteToCSV(newFlights, discontinuedFlights);
           
            DateTime endTime = DateTime.Now;          
            TimeSpan executionTime = endTime - startTime;
            Console.WriteLine($"Execution time: {executionTime.TotalMilliseconds} ms");
           
        }
        private List<ResultViewModel> CheckNewFlights(DateTime startDate, DateTime endDate)
        {
            Console.WriteLine("Start : Check New Flights.");

            
            DateTime previousWeek = startDate.AddDays(-7).AddMinutes(-30);
            DateTime nextWeek = endDate.AddMinutes(30);

            var newFlights = dbContext.Flights
                .Join(dbContext.Routes,
                flight => flight.route_id,
                route => route.route_id,(flight, route) => new
                {
                    Flights = flight,
                    Routes  = route,
                })
                .Where(f => f.Flights.departure_time >= previousWeek && f.Flights.departure_time <= nextWeek).
                Select(f => new ResultViewModel
                {
                    airline_id=f.Flights.airline_id,
                    flight_id=f.Flights.flight_id,
                    origin_city_id=f.Routes.origin_city_id,
                    destination_city_id=f.Routes.destination_city_id,
                    departure_time=f.Flights.departure_time,
                    arrival_time=f.Flights.arrival_time

                })
                .ToList();

            Console.WriteLine("End : Check New Flights.");

            return newFlights;
        }
        private List<ResultViewModel> CheckDiscontinuedFlights(DateTime startDate, DateTime endDate)
        {
            Console.WriteLine("Start : Check Discontinued Flights.");
            DateTime previousWeek = startDate.AddMinutes(-30);
            DateTime nextWeek = endDate.AddDays(7).AddMinutes(30);

            var discontinuedFlights = dbContext.Flights
                                .Join(dbContext.Routes,
                flight => flight.route_id,
                route => route.route_id, (flight, route) => new
                {
                    Flights = flight,
                    Routes = route,
                })
                .Where(f => f.Flights.departure_time >= previousWeek && f.Flights.departure_time <= nextWeek).
                                Select(f => new ResultViewModel
                                {
                                    airline_id = f.Flights.airline_id,
                                    flight_id = f.Flights.flight_id,
                                    origin_city_id = f.Routes.origin_city_id,
                                    destination_city_id = f.Routes.destination_city_id,
                                    departure_time = f.Flights.departure_time,
                                    arrival_time = f.Flights.arrival_time

                                })
                .ToList();

            Console.WriteLine("End : Check Discontinued Flights.");

            return discontinuedFlights;
        }       
        private void WriteToCSV(List<ResultViewModel> newFlights, List<ResultViewModel> discontinuedFlights)
        {
                using (StreamWriter writer = new StreamWriter("results.csv"))
                {

                    Console.WriteLine("Start : Adding result into results.csv.");

                    writer.WriteLine("flight_id,origin_city_id,destination_city_id,departure_time,arrival_time,airline_id,status");

                    foreach (var flight in newFlights)
                    {
                        if (flight.arrival_time != Convert.ToDateTime("31-12-9999  23:59:00"))
                        {

                            writer.WriteLine($"{flight.flight_id},{flight.origin_city_id},{flight.destination_city_id},{flight.departure_time},{flight.arrival_time.AddHours(2)},{flight.airline_id},New");
                        }
                    }

                    foreach (var flight in discontinuedFlights)
                    {
                        if (flight.arrival_time != Convert.ToDateTime("31-12-9999  23:59:00"))
                        {
                            writer.WriteLine($"{flight.flight_id},{flight.origin_city_id},{flight.destination_city_id},{flight.departure_time},{flight.arrival_time.AddHours(2)},{flight.airline_id},Discontinued");
                        }

                    }

                    Console.WriteLine("End : Adding result into results.csv.");
                    Console.WriteLine("Airlines data added to results.csv");
                }
        }
    }
}
