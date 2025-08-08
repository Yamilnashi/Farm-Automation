using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FarmToTableWebApp.Controllers.api
{
    [Route("api/v1")]
    [ApiController]
    public class FarmToTableApiController : ControllerBase
    {
        #region Fields
        private readonly ILogger<FarmToTableApiController> _logger;
        private readonly IDboDbContextWrite _dbContext;
        #endregion

        #region Constructor
        public FarmToTableApiController(ILogger<FarmToTableApiController> logger,
            IDboDbContextWrite dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        #endregion

        [HttpPut]
        [Route("pending-analysis")]
        public async Task<IActionResult> PutPendingResult([FromBody] string dto)
        {
            PendingAnalysisDTO? obj = JsonConvert.DeserializeObject<PendingAnalysisDTO?>(dto);
            if (obj != null)
            {
                MoistureReadingHistoryChange? model = JsonConvert.DeserializeObject<MoistureReadingHistoryChange>(obj.JsonData);
                _logger.LogInformation($"Received PUT Request containing: {obj.JsonData}");
            }
            
            return NoContent();
        }
    }
}
