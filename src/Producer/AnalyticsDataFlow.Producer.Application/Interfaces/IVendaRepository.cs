using AnalyticsDataFlow.Producer.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.Application.Interfaces
{
    public interface IVendaRepository
    {
        Task<IEnumerable<Venda>> ObterVendasPorDia(int diaIncremento, DateTime dataPrimeiroRegistro);
        Task<IEnumerable<Produto>> ObterProdutos();
        Task<IEnumerable<VendaDTO>> ObterVendasNoSql(int diaIncremento, DateTime dataPrimeiroRegistro);
    }
}
