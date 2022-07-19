using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.AnalyticsProducer.Application.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public int ReferenciaIntegracaoId { get; set; }
        public int ReferenciaIntegracaoItemId { get; set; }
        public int ProdutoId { get; set; }
        public int FilialId { get; set; }
        public int FuncionarioId { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public int Quantidade { get; set; }
        public string Origem { get; set; }
        public bool ProdutoVencendo { get; set; }
        public string FormaPagamento { get; set; }
        public decimal Custo { get; set; }
        public decimal CustoFinal { get; set; }
        public decimal CustoMedio { get; set; }
        public decimal Desconto { get; set; }
        public string CodigoBarras { get; set; }
        public string TipoPbm { get; set; }
        public string ValorUnitario { get; set; }
        public string Plataforma { get; set; }
        public DateTime DataAlteracao { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool FoiRemovido { get; set; }
    }
}
