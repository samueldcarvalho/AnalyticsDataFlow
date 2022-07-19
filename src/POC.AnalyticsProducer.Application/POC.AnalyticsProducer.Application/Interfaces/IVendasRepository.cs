using POC.AnalyticsProducer.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.AnalyticsProducer.Application.Interfaces
{
    public interface IVendasRepository
    {
        Task<IEnumerable<Venda>> ObterVendas(int diaIncremento);
    }
}
