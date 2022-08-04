using AnalyticsDataFlow.Producer.Application.Interfaces;
using AnalyticsDataFlow.Producer.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.Application.Services
{
    public class ProducerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProducerService> _logger;
        private readonly HttpClient _httpClient;
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
            _httpClient = new HttpClient();
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
            IEnumerable<Venda> vendas = await _vendaRepository.ObterVendasPorDia(_counter, new DateTime(2020, 11, 01));

            foreach (var venda in vendas)
            {
                var json = JsonConvert.SerializeObject(venda);
                var content = new StringContent(json);
                content.Headers.ContentType.MediaType = MediaTypeNames.Application.Json;
                try
                {
                    var response = await _httpClient.PostAsync($"http://localhost:9200/vendas/_doc/{venda.Id}", content);
                    Console.WriteLine($"Sucesso! Venda {venda.Id}" + _counter);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            _counter++;
        }
    }
}
