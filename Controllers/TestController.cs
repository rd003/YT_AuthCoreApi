using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthCoreApi.Models;
using AutoMapper;
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
        private readonly IMapper _mapper;
        public TestController(ILogger<TestController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
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

        [HttpGet]
        public IActionResult TestAutomapper()
        {
            var empDTO = new EmployeeDTO
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@xyz.com",
                DepartmentId = 1,
                DepartmentName = "Sales"
            };
            var emp = _mapper.Map<Employee>(empDTO);
            return Ok(emp);
        }
    }
}
