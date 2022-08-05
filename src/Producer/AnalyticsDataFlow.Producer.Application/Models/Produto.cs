using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.Application.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int ReferenciaIntegracaoId { get; set; }
        public int ProdutoCategoriaId { get; set; }
        public int ProdutoSubgrupoId { get; set; }
        public int ProdutoGrupoPrincipalId { get; set; }
        public int ProdutoLaboratorioId { get; set; }
        public int AgrupamentoProdutoId { get; set; }
        public int AgrupamentoProdutoCampanhaId { get; set; }
        public DateTime DataAlteracao { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool FoiRemovido { get; set; }
    }

    public class JoinElasticChildren
    {
        public string Name { get; set; }
        public string Parent { get; set; }
    }
}
