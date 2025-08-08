using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using FarmToTableWebApp.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Transactions;

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

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Test()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNewPendingAnalysis", EEventType.Moisture, new MoistureReadingHistoryChange()
            {
                SentinelId = 1,
                Operation = ECdcChangeType.Insert,
                Moisture = (byte)new Random().Next(1, 10),
                MoistureReadingHistoryId = 1,
                SavedDate = DateTime.UtcNow
            });
            return Ok();
        }

        #region Private
        private async Task PutModelData(PendingAnalysisDTO dtoModel)
        {
            int analysisId = await _dbContext.AnalysisSave(dtoModel.SentinelId, dtoModel.InstanceId, dtoModel.SavedDate);

            if (dtoModel.EventType == EEventType.Moisture)
            {
                MoistureReadingHistoryChange model = JsonConvert.DeserializeObject<MoistureReadingHistoryChange>(dtoModel.JsonData)!;
                await _dbContext.MoistureAnalysisSave(analysisId, model.Moisture, model.SavedDate);
            }
            else if (dtoModel.EventType == EEventType.Temperature)
            {
                TemperatureReadingHistoryChange model = JsonConvert.DeserializeObject<TemperatureReadingHistoryChange>(dtoModel.JsonData)!;
                await _dbContext.TemperatureAnalysisSave(analysisId, model.TemperatureCelsius, model.SavedDate);
            }
            else if (dtoModel.EventType == EEventType.Soil)
            {
                SoilReadingHistoryChange model = JsonConvert.DeserializeObject<SoilReadingHistoryChange>(dtoModel.JsonData)!;
                await _dbContext.SoilAnalysisSave(analysisId, model.NPpm, model.PPpm, model.KPpm, model.SavedDate);
            }
            else if (dtoModel.EventType == EEventType.SentinelStatus)
            {
                SentinelStatusHistoryChange model = JsonConvert.DeserializeObject<SentinelStatusHistoryChange>(dtoModel.JsonData)!;
                await _dbContext.SentinelStatusAnalysisSave(analysisId, (int)model.SentinelStatusCode, model.SavedDate);
            }

            await _hubContext.Clients.All.SendAsync("ReceiveNewPendingAnalysis", dtoModel.EventType, dtoModel.JsonData);
            _logger.LogInformation($"Received PUT request for {dtoModel.EventType}. Now Pending Analysis.");
        }
        #endregion
    }
}
