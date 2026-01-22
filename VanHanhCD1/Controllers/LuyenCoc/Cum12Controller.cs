using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.DTOs.Response;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.LuyenCoc
{
    [Route("api/[controller]")]
    [ApiController]
    public class Cum12Controller : ControllerBase
    {
        private readonly ILuyenCocRepository _service;
        public Cum12Controller(ILuyenCocRepository service)
        {
            _service = service;
        }
        [HttpGet("")]
        public  ApiResponse<IEnumerable<Dictionary<string, object>>> SearchCum12([FromQuery] DateTime? time)
        {
            var data = _service.GetSearchTimeLuyenCocCum12(time);
            return ApiResponse<IEnumerable<Dictionary<string, object>>>.SuccessResponse(data);
        }
    }
}
