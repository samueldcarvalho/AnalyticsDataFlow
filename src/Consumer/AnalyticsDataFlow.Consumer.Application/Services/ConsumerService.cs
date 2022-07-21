using AnalyticsDataFlow.Producer.Application.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Spark;
using Microsoft.Spark.Sql;
using Newtonsoft.Json;
using Raven.Client.Documents;
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
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "Venda.Consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var dbConnection = new DocumentStore
            {
                Urls = new[] { "http://localhost:8080" },
                Database = "analytics"
            }.Initialize();

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("venda_topic");

                using (var dbSession = dbConnection.OpenSession())
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"{DateTime.Now.Hour:00}:{DateTime.Now.Minute:00}:{DateTime.Now.Second:00}:{DateTime.Now.Millisecond:000}");

                        ConsumeResult<Ignore, string> consumeResult = consumer.Consume(stoppingToken);

                        var value = consumeResult?.Message?.Value;

                        if (string.IsNullOrWhiteSpace(value))
                            continue;

                        var vendas = JsonConvert.DeserializeObject<List<VendaViewModel>>(value);

                        vendas.ForEach(v => dbSession.Store(v));

                        dbSession.SaveChanges();
                    }

                await Task.Delay(5000, stoppingToken);
                consumer.Close();
            }
        }
    }
}


