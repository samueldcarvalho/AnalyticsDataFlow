using AnalyticsDataFlow.Producer.Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.Application.Services
{
    public class ProducerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProducerService> _logger;
        private IVendaRepository _vendaRepository;

        private int _counter = 0;

        public ProducerService(IServiceProvider serviceProvider, ILogger<ProducerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                _vendaRepository = scope.ServiceProvider.GetRequiredService<IVendaRepository>();

                await Produce(stoppingToken);
            }
        }

        private async Task Produce(CancellationToken cancellationToken)
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

            using (var prod = new ProducerBuilder<Null, string>(config).Build())
            {
                while (true)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        var result = await prod.ProduceAsync("topico_vendas", new Message<Null, string>() { Value = _counter.ToString() });
                        Console.WriteLine($"Enviando: '{result.Value}' para '{result.TopicPartitionOffset}' | {_counter}");

                        _counter++;

                        await Task.Delay(5000);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }
            }
        }
    }
}
