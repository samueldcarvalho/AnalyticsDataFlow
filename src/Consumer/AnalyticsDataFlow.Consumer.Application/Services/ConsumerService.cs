using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Spark;
using Microsoft.Spark.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Consumer.Application.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly ILogger<ConsumerService> _logger;

        public ConsumerService(ILogger<ConsumerService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var spark = SparkSession.Builder().GetOrCreate();

            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                var df = spark.ReadStream()
                    .Format("kafka")
                    .Option("kafka.bootstrap.servers", "localhost:9092")
                    .Option("subscribe", "venda_topic")
                    .Option("startingOffsets", "earliest")
                    .Load();

                df.PrintSchema();

                spark.Stop();
            }   
        }
    }
}
