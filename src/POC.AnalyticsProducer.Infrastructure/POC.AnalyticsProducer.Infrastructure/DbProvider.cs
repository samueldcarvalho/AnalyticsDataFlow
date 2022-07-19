using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.AnalyticsProducer.Infrastructure
{
    public abstract class DbRepository
    {
        protected readonly MySqlConnection _connection;

        public DbRepository(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("Connection String não informada para comunicação com o banco de dados.");

            _connection = new MySqlConnection(connectionString);
        }
    }
}
