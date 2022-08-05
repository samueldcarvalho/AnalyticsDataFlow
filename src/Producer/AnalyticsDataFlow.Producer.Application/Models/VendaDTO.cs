using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.Application.Models
{
    public class VendaDTO
    {
        public VendaDTO()
        {
            vendas_join = new JoinElastic
            {
                Name = "vendas"
            };
        }
        public string VendaId { get; set; }
        public int ProdutoId { get; set; }
        public int FilialId { get; set; }
        public int FuncionarioId { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public int Quantidade { get; set; }
        public decimal CustoFinal { get; set; }
        public decimal Desconto { get; set; }
        public decimal ValorUnitario { get; set; }
        public JoinElastic vendas_join { get; set; }
    }

    public class JoinElastic {
        public string Name { get; set; }
    }
}
