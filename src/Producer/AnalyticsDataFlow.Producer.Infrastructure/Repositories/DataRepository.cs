using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.Infrastructure.Repositories
{
    public abstract class DataRepository
    {
        protected readonly MySqlConnection _connection;

        public DataRepository(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("ConnectionString não declarada no AppSettings");

            _connection = new MySqlConnection(connectionString);
        }
    }
}
