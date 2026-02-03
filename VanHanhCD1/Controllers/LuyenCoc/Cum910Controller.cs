using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.DTOs.Response;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.LuyenCoc
{
    [Route("api/[controller]")]
    [ApiController]
    public class Cum910Controller : ControllerBase
    {
        private readonly ILuyenCocRepository _service;
        public Cum910Controller(ILuyenCocRepository service)
        {
            _service = service;
        }
        [HttpGet()]
        public ApiResponse<IEnumerable<Dictionary<string, object>>> SearchCum910([FromQuery] DateTime? time)
        {
            var data = _service.GetSearchTimeLuyenCocCum910(time);
            return ApiResponse<IEnumerable<Dictionary<string, object>>>.SuccessResponse(data);
        }
    }
}
