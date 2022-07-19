using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POC.AnalyticsProducer.Application.Interfaces;
using POC.AnalyticsProducer.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace POC.AnalyticsProducer.Application.Services
{
    public class ProducerService : BackgroundService
    {
        private int _counter = 0;

        private readonly ILogger<ProducerService> _logger;
        private readonly IServiceProvider _services;

        private IVendasRepository _vendasRepository;

        public ProducerService(ILogger<ProducerService> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _services.CreateScope())
            {
                _vendasRepository = scope.ServiceProvider.GetRequiredService<IVendasRepository>();

                await Produce(stoppingToken);
            }
        }

        protected async Task Produce(CancellationToken stoppingToken)
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

            using (var prod = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    while (true)
                    {
                        if (stoppingToken.IsCancellationRequested)
                            break;

                        IEnumerable<Venda> vendas = await _vendasRepository.ObterVendas(_counter);

                        if (vendas.Any())
                        {
                            vendas.ToList().AsParallel().ForAll(async v => 
                            { 
                                var dr = await prod.ProduceAsync("topico_vendas", new Message<Null, string>() { Value = JsonConvert.SerializeObject(v)}, stoppingToken);
                                Console.WriteLine($"Enviando: '{v.Id}' para '{dr.TopicPartitionOffset}' | {_counter}");
                            });
                        }

                        _counter++;

                        await Task.Delay(5000);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{DateTime.Now} - Kafka Producer - {ex.Message}");
                }
            }
        }
    }
}
