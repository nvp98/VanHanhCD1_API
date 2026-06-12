using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.DTOs.Response;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.LuyenCoc
{
    [Route("api/[controller]")]
    [ApiController]
    public class Cum34Controller : ControllerBase
    {
        private readonly ILuyenCocRepository _service;
        public Cum34Controller(ILuyenCocRepository service)
        {
            _service = service;
        }
        [HttpGet()]
        public ApiResponse<IEnumerable<Dictionary<string, object>>> SearchCum34([FromQuery] DateTime? time)
        {
            var data = _service.GetSearchTimeLuyenCocCum34(time);
            return ApiResponse<IEnumerable<Dictionary<string, object>>>.SuccessResponse(data);
        }
    }
}
