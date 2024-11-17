using Microsoft.AspNetCore.Mvc;
using RedisSqlDemo.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisSqlDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly DataService _dataService;

        public DataController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("sql")]
        public async Task<IActionResult> GetSqlData() // Changed to IActionResult
        {
            var data = await _dataService.GetDataFromSqlAsync();
            return Ok(data);
        }

        [HttpGet("redis")]
        public async Task<IActionResult> GetRedisData() // Changed to IActionResult
        {
            var data = await _dataService.GetDataFromRedisAsync();
            return Ok(data);
        }
    }
}