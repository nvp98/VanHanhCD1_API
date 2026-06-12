using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.DTOs.Response;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.LuyenCoc
{
    [Route("api/[controller]")]
    [ApiController]
    public class Cum78Controller : ControllerBase
    {
        private readonly ILuyenCocRepository _service;
        public Cum78Controller(ILuyenCocRepository service)
        {
            _service = service;
        }
        [HttpGet()]
        public ApiResponse<IEnumerable<Dictionary<string, object>>> SearchCum78([FromQuery] DateTime? time)
        {
            var data = _service.GetSearchTimeLuyenCocCum78(time);
            return ApiResponse<IEnumerable<Dictionary<string, object>>>.SuccessResponse(data);
        }
    }
}
