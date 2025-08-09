using FarmToTableData.Extensions;
using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using FarmToTableWebApp.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<AnalysisHub> _hubContext;
        #endregion

        #region Constructor
        public FarmToTableApiController(ILogger<FarmToTableApiController> logger,
            IDboDbContextWrite dbContext,
            IHubContext<AnalysisHub> hubContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _hubContext = hubContext;
        }
        #endregion

        #region PUT
        [HttpPut]
        [Route("pending-analysis")]
        public async Task<IActionResult> PutPendingResult([FromBody] string dto)
        {
            PendingAnalysisDTO? dtoModel = JsonConvert.DeserializeObject<PendingAnalysisDTO?>(dto);
            if (dtoModel != null &&
                dtoModel.EventType != EEventType.Sentinel) // not handling these yet
            {
                await PutModelData(dtoModel);
            }
            return NoContent();
        }
        #endregion

        #region Private
        private async Task PutModelData(PendingAnalysisDTO dtoModel)
        {
            int analysisId = await _dbContext.AnalysisSave(dtoModel.SentinelId, dtoModel.SavedDate);

            if (dtoModel.EventType == EEventType.Moisture)
            {
                MoistureReadingHistoryChange model = JsonConvert.DeserializeObject<MoistureReadingHistoryChange>(dtoModel.JsonData)!;
                await _dbContext.MoistureAnalysisSave(analysisId, dtoModel.InstanceId, model.Moisture, model.SavedDate);
            }
            else if (dtoModel.EventType == EEventType.Temperature)
            {
                TemperatureReadingHistoryChange model = JsonConvert.DeserializeObject<TemperatureReadingHistoryChange>(dtoModel.JsonData)!;
                await _dbContext.TemperatureAnalysisSave(analysisId, dtoModel.InstanceId, model.TemperatureCelsius, model.SavedDate);
            }
            else if (dtoModel.EventType == EEventType.Soil)
            {
                SoilReadingHistoryChange model = JsonConvert.DeserializeObject<SoilReadingHistoryChange>(dtoModel.JsonData)!;
                await _dbContext.SoilAnalysisSave(analysisId, dtoModel.InstanceId, model.NPpm, model.PPpm, model.KPpm, model.SavedDate);
            }
            else if (dtoModel.EventType == EEventType.SentinelStatus)
            {
                SentinelStatusHistoryChange model = JsonConvert.DeserializeObject<SentinelStatusHistoryChange>(dtoModel.JsonData)!;
                await _dbContext.SentinelStatusAnalysisSave(analysisId, dtoModel.InstanceId, (int)model.SentinelStatusCode, model.SavedDate);
            }

            await _hubContext.Clients.All.SendAsync("ReceiveNewPendingAnalysis", dtoModel.EventType, dtoModel.JsonData);
            _logger.LogInformation($"Received PUT request for {dtoModel.EventType}. Now Pending Analysis.");
        }
        #endregion
    }
}
