using Dapper;
using Microsoft.Extensions.Configuration;
using POC.AnalyticsProducer.Application.Interfaces;
using POC.AnalyticsProducer.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.AnalyticsProducer.Infrastructure.Repository
{
    public class VendasRepository : DbRepository, IVendasRepository
    {
        public VendasRepository(IConfiguration configuration) : base(configuration) { }

        public Task<IEnumerable<Venda>> ObterVendas(int diaIncremento)
        {
            var dataInicial = new DateTime(2020, 11, 1);

            var dataFiltro = dataInicial.AddDays(diaIncremento);

            return _connection.QueryAsync<Venda>($@"SELECT 
                                        id Id,
                                        referencia_integracao_id ReferenciaIntegracaoId,
                                        referencia_integracao_item_id ReferenciaIntegracaoItemId,
                                        produto_id ProdutoId,
                                        filial_id FilialId,
                                        funcionario_id FuncionarioId,
                                        data_venda DataVenda,
                                        valor_total ValorTotal,
                                        quantidade Quantidade,
                                        origem Origem,
                                        produto_vencendo ProdutoVencendo,
                                        forma_pagamento FormaPagamento,
                                        custo Custo,
                                        custo_final CustoFinal,
                                        custom_medio CustoMedio,
                                        desconto Desconto,
                                        codigo_barras CodigoBarras,
                                        tipo_pbm TipoPbm,
                                        valor_unitario ValorUnitario,
                                        plataforma Plataforma,
                                        data_alteracao DataAlteracao,
                                        data_criacao DataCriacao,
                                        foi_removido FoiRemovido
                                    FROM
                                        venda
                                    WHERE 
	                                    data_venda BETWEEN '{dataFiltro:yyyy-MM-dd 00:00:00}' AND '{dataFiltro:yyyy-MM-dd 23:59:59}'");
        }
    }
}
