using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAgencies.EntityFrameWork
{
    public class AirlineAgenciesDbContext:DbContext
    {
        public AirlineAgenciesDbContext() : base(GetConnectionString())
        {
        }

        public DbSet<Flights> Flights { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<Subscriptions> Subscriptions { get; set; }

        private static string GetConnectionString()
        {
            // Retrieve the connection string from app.config
            string connectionString = ConfigurationManager.AppSettings["AirlineAgenciesDb"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in appSettings.");
            }

            return connectionString;
        }
    }
}
