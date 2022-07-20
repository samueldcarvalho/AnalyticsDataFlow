using AnalyticsDataFlow.Producer.Application.Interfaces;
using AnalyticsDataFlow.Producer.Application.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.Application.Services
{
    public class ProducerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProducerService> _logger;
        private IVendaRepository _vendaRepository;

        private readonly string _brokerURL;
        private int _counter = 0;

        public ProducerService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ProducerService> logger)
        {
            _brokerURL = configuration.GetSection("Kafka:ServerURL").Value;

            if (string.IsNullOrWhiteSpace(_brokerURL))
                throw new Exception("BrokerURL não foi informado nas varíaveis de ambiente.");

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
            var config = new ProducerConfig { BootstrapServers = _brokerURL };

            using (var prod = new ProducerBuilder<string, string>(config).Build())
            {
                while (true)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        IEnumerable<Venda> vendas = await _vendaRepository.ObterVendasPorDia(_counter, new DateTime(2020,11,01));

                        if (vendas.Any())
                        {
                            _logger.Log(LogLevel.Information, $"\nPreparação de {vendas.Count()} para envio ao tópico do Kafka\n");

                            var pacotesVendas = vendas.Select((value, index) => new { Index = index, Value = value })
                                .GroupBy(g => g.Index / 1000)
                                .Select(g => g.Select(x => x.Value));

                            foreach (IEnumerable<Venda> pacote in pacotesVendas)
                            {
                                var result = await prod.ProduceAsync("venda_topic", new Message<string, string>() { Key= $"{pacote.Min(v => v.Id)}{pacote.Max(v => v.Id)}", Value = JsonConvert.SerializeObject(pacote)});;
                                Console.WriteLine($"Pacote [{pacote.Min(v => v.Id)}-{pacote.Max(v => v.Id)}] para '{result.TopicPartitionOffset}' | DataPacote: {new DateTime(2020, 11, 01).AddDays(_counter)}");
                            }
                        }

                        _counter++;

                        await Task.Delay(10000);
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
