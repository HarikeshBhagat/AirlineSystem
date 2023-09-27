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

            var newFlights = DetectNewFlights(startDate, endDate);
            var discontinuedFlights = DetectDiscontinuedFlights(startDate, endDate);

            WriteToCSV(newFlights, discontinuedFlights);
            // Measure the end time
            DateTime endTime = DateTime.Now;

            // Calculate and display execution time
            TimeSpan executionTime = endTime - startTime;
            Console.WriteLine($"Execution time: {executionTime.TotalMilliseconds} ms");
            Console.WriteLine("Results written to results.csv");
        }

        private List<ResultViewModel> DetectNewFlights(DateTime startDate, DateTime endDate)
        {
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

            return newFlights;
        }

        private List<ResultViewModel> DetectDiscontinuedFlights(DateTime startDate, DateTime endDate)
        {

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

            return discontinuedFlights;
        }
        private DateTime EnsureValidDateTime(DateTime inputDateTime)
        {
            if (inputDateTime < DateTime.MinValue)
                return DateTime.MinValue;
            else if (inputDateTime > DateTime.MaxValue)
                return DateTime.MaxValue;
            else
                return inputDateTime;
        }
        private void WriteToCSV(List<ResultViewModel> newFlights, List<ResultViewModel> discontinuedFlights)
        {
                using (StreamWriter writer = new StreamWriter("results.csv"))
                {
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
                }
        }
    }
}
