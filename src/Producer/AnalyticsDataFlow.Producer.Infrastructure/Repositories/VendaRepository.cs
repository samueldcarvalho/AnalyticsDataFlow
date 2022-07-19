using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.Infrastructure.Repositories
{
    public class VendaRepository : DataRepository
    {
        public VendaRepository(IConfiguration configuration) : base(configuration) { }
    }
}
