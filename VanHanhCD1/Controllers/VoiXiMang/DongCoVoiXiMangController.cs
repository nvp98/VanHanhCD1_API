using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.VoiXiMang
{
    [Route("api/[controller]")]
    [ApiController]
    public class DongCoVoiXiMangController : ControllerBase
    {
        private readonly IVoiXiMangRepository _service;
        public DongCoVoiXiMangController(IVoiXiMangRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursDongCoVoiXiMangs()
        {
            var result = await _service.GetLast24HoursDongCoVoiXiMang();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeDongCoVoiXiMangs([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeDongCoVoiXiMang(from, to);
            return Ok(result);
        }

        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _service.GetDongCoVoiXiMangMinValues();
            return result;
        }
    }
}
