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
            //List<Produto> produtos = (await _vendaRepository.ObterProdutos()).ToList();
            //produtos.ForEach(async p =>
            //{
            //    var json = JsonConvert.SerializeObject(p);
            //    var content = new StringContent(json);
            //    content.Headers.ContentType.MediaType = MediaTypeNames.Application.Json;

            //    try
            //    {
            //        var response = await _httpClient.PostAsync($"http://dev001.softwaredrogaria.com.br:9200/produtos/_doc/{p.Id}", content);
            //        Console.WriteLine($"Sucesso! Produto {p.Id}" );
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex.Message);
            //        return;
            //    }
            //});

            while (!cancellationToken.IsCancellationRequested)
            {
                IEnumerable<VendaDTO> vendas = await _vendaRepository.ObterVendasNoSql(_counter, new DateTime(2020, 11, 01));

                var pacotesVendas = vendas
                    .Select((venda, index) => new { Venda = venda, Index = index })
                    .GroupBy(g => g.Index / 200).Select(g => g.Select(v => v.Venda)).ToList();

                pacotesVendas.ForEach(async pacote => {
                    foreach (var venda in pacote)
                    {
                        var json = JsonConvert.SerializeObject(venda);
                        var content = new StringContent(json);
                        content.Headers.ContentType.MediaType = MediaTypeNames.Application.Json;

                        try
                        {
                            var response = await _httpClient.PostAsync($"http://dev001.softwaredrogaria.com.br:9200/vendas/_doc/{venda.VendaId}", content);
                            Console.WriteLine($"Sucesso! Venda {venda.VendaId}" + _counter);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                    }
                });

                _counter++;
            }
        }
    }
}
