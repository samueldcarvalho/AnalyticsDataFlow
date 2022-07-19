using AnalyticsDataFlow.Producer.Application.Interfaces;
using AnalyticsDataFlow.Producer.Application.Models;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            var config = new ProducerConfig { BootstrapServers = "dev001.softwaredrogaria.com.br:9000" };

            using (var prod = new ProducerBuilder<Null, string>(config).Build())
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
                                var result = await prod.ProduceAsync("topico_vendas", new Message<Null, string>() { Value = JsonConvert.SerializeObject(pacote)});
                                Console.WriteLine($"Pacote [{pacote.Min(v => v.Id)}-{pacote.Max(v => v.Id)}] para '{result.TopicPartitionOffset}' | DataPacote: {new DateTime(2020, 11, 01).AddDays(_counter)}");
                            }
                        }

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
