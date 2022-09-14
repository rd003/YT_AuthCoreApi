using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthCoreApi.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        public IActionResult TestDivideByZero()
        {
            try
            {
                int n = 12;
                int sum = n / 0;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error on test controller, TestDivideByzeroMethod");
            }
            return Ok();
        }
    }
}
