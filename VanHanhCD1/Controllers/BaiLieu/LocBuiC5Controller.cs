using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.BaiLieu
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocBuiC5Controller : ControllerBase
    {
        private readonly IBaiLieuRepository _service;
        public LocBuiC5Controller(IBaiLieuRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLocBuiC5s()
        {
            var result = await _service.GetLast24HoursDongCoLocBuiC5s();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeLocBuiC5s([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeDongCoLocBuiC5s(from, to);
            return Ok(result);
        }
        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _service.GetDongCoLocBuiC5MinValues();
            return result;
        }
    }
}
