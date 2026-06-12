using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.BaiLieu
{
    [Route("api/[controller]")]
    [ApiController]
    public class MBABaiLieuController : ControllerBase
    {
        private readonly IBaiLieuRepository _service;
        public MBABaiLieuController(IBaiLieuRepository service)
        {
            _service = service;
        }

        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursMBABaiLieu()
        {
            var result = await _service.GetLast24HoursMBABaiLieus();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeMBABaiLieus([FromQuery] DateTime from, [FromQuery] DateTime to) 
        {
            var result = await _service.GetSearchTimeMBABaiLieus(from, to);
            return Ok(result);
        }
        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _service.GetMBABaiLieuMinValues();
            return result;
        }
    }
}
