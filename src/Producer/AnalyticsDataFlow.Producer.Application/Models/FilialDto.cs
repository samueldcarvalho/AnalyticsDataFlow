using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.Application.Models
{
    class FilialDto
    {
        public FilialDto()
        {
            filial_join = new() { Name = "filial_join" };
        }
        public int Id { get; set; }
        public string NomeFantasia { get; set; }
        public string NomeRazao { get; set; }
        public JoinElastic filial_join { get; set; }
    }
}
