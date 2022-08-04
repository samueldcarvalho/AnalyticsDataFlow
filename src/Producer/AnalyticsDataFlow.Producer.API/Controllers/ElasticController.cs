using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyticsDataFlow.Producer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElasticController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;

        public ElasticController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
    }
}
