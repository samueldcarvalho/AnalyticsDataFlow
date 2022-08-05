using AnalyticsDataFlow.Producer.Application.Interfaces;
using AnalyticsDataFlow.Producer.Application.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.Infrastructure.Repositories
{
    public class VendaRepository : DataRepository, IVendaRepository
    {
        public VendaRepository(IConfiguration configuration) : base(configuration) { }

        public Task<IEnumerable<Produto>> ObterProdutos()
        {
            return _connection.QueryAsync<Produto>($@"
                                                    SELECT 
                                                        id Id,
                                                        descricao Descricao,
                                                        referencia_integracao_id ReferenciaIntegracaoId,
                                                        produto_categoria_id ProdutoCategoriaId,
                                                        produto_subgrupo_id ProdutoSubgrupoId,
                                                        produto_grupo_principal_id ProdutoGrupoPrincipalId, 
                                                        produto_laboratorio_id ProdutoLaboratorioId, 
                                                        agrupamento_produto_id AgrupamentoProdutoId, 
                                                        agrupamento_produto_campanha_id AgrupamentoProdutoCampanhaId, 
                                                        data_alteracao DataAlteracao, 
                                                        data_criacao DataCriacao, 
                                                        foi_removido FoiRemovido
                                                    FROM drogarias_minasmais.produto;    
                                                    ");
        }

        public Task<IEnumerable<VendaDTO>> ObterVendasNoSql(int diaIncremento, DateTime dataPrimeiroRegistro)
        {
            var data = dataPrimeiroRegistro.AddDays(diaIncremento);

            return _connection.QueryAsync<VendaDTO>($@"SELECT 
                                                            CONCAT(filial_id,'-',v.referencia_integracao_id) VendaId,
                                                            CONCAT(filial_id,'-',v.referencia_integracao_item_id) ItemId,
                                                            v.produto_id ProdutoId,
                                                            filial_id FilialId,
                                                            funcionario_id FuncionarioId,
                                                            data_venda DataVenda,
                                                            valor_total ValorTotal,
                                                            quantidade Quantidade,
                                                            custo_final CustoFinal,
                                                            desconto Desconto,
                                                            valor_unitario ValorUnitario
                                                            FROM venda v
                                                        WHERE 
    	                                                    data_venda BETWEEN '{data:yyyy-MM-dd 00:00:00}' AND '{data:yyyy-MM-dd 23:59:59}' ;");
        }


        public Task<IEnumerable<Venda>> ObterVendasPorDia(int diaIncremento, DateTime dataPrimeiroRegistro)
        {
            var data = dataPrimeiroRegistro.AddDays(diaIncremento);

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
	                                                    data_venda BETWEEN '{data:yyyy-MM-dd 00:00:00}' AND '{data:yyyy-MM-dd 23:59:59}' ;");
        }
    }
}
