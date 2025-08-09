using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using FarmToTableWebApp.Services;
using FarmToTableWebApp.ViewModels.Sentinels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FarmToTableWebApp.Controllers
{
    [Route("Sentinel")]
    public class SentinelController : Controller
    {
        #region Fields
        private readonly ILogger<SentinelController> _logger;
        private readonly IDboDbContextRead _dbContextRead;
        private readonly IDboDbContextWrite _dbContextWrite;
        private readonly WebAppClient _client;
        #endregion

        #region Constructor
        public SentinelController(ILogger<SentinelController> logger,
            IDboDbContextRead dbContextRead,
            IDboDbContextWrite dbContextWrite,
            WebAppClient client)
        {
            _logger = logger;
            _dbContextRead = dbContextRead;
            _dbContextWrite = dbContextWrite;
            _client = client;
        }
        #endregion

        #region GET
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            SentinelIndexViewModel model = new SentinelIndexViewModel();
            return View(model);
        }

        [HttpGet]
        [Route("AnalysisTableJson")]
        public async Task<IActionResult> AnalysisTableJson(bool? isAnalyzed = false)
        {
            string json = string.Empty;
            try
            {
                (IEnumerable<AnalysisBase> analyses,
                    IEnumerable<SoilAnalysis> soils, 
                    IEnumerable<MoistureAnalysis> moistures, 
                    IEnumerable<TemperatureAnalysis> temperatures, 
                    IEnumerable<SentinelStatusAnalysis> statuses) = await _dbContextRead.AnalysisList(isAnalyzed);

                Dictionary<int, AnalysisBase> analysesDict = analyses
                    .GroupBy(x => x.AnalysisId)
                    .ToDictionary(x => x.Key, x => x.First());

                Dictionary<int, SoilAnalysis?> soilsDict = soils
                    .GroupBy(x => x.AnalysisId)
                    .ToDictionary(x => x.Key, x => x?.First());

                Dictionary<int, MoistureAnalysis?> moisturesDict = moistures
                    .GroupBy(x => x.AnalysisId)
                    .ToDictionary(x => x.Key, x => x?.First());

                Dictionary<int, TemperatureAnalysis?> temperaturesDict = temperatures
                    .GroupBy(x => x.AnalysisId)
                    .ToDictionary(x => x.Key, x => x?.First());

                Dictionary<int, SentinelStatusAnalysis?> statusesDict = statuses
                    .GroupBy(x => x.AnalysisId)
                    .ToDictionary(x => x.Key, x => x?.First());

                List<AnalysisTableItemViewModel> models = new List<AnalysisTableItemViewModel>();
                foreach (KeyValuePair<int, AnalysisBase> kvp in analysesDict)
                {
                    int analysisId = kvp.Key;
                    AnalysisBase analysis = kvp.Value;
                    AnalysisTableItemViewModel model = new AnalysisTableItemViewModel()
                    {
                        AnalysisId = analysisId,
                        SentinelId = analysis.SentinelId,
                        IsAnalyzed = analysis.IsAnalyzed,
                        SavedDate = analysis.SavedDate,
                        AnalysisDataDict = GetAnalysisDataDicts(analysisId, soilsDict, moisturesDict, temperaturesDict, statusesDict)
                    };
                    models.Add(model);
                }
                json = JsonConvert.SerializeObject(new { data = models });
            } catch (Exception ex)
            {
                _logger.LogError($"An error occurred trying to get analysis data for table: {ex.Message}");
            }
            return Content(json, "application/json");
        }
        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SubmitAnalysisResult")]
        public async Task<IActionResult> SubmitAnalysisResult(int analysisId)
        {
            Analysis analysis = await _dbContextRead.AnalysisGet(analysisId);
            if (analysis == null)
            {
                return NotFound();
            }

            Dictionary<string, EEventType> analysisToSendDict = new Dictionary<string, EEventType>();
            if (!string.IsNullOrEmpty(analysis.SoilInstanceId))
            {
                analysisToSendDict[analysis.SoilInstanceId] = EEventType.Soil;
            }
            if (!string.IsNullOrEmpty(analysis.MoistureInstanceId))
            {
                analysisToSendDict[analysis.MoistureInstanceId] = EEventType.Moisture;
            }
            if (!string.IsNullOrEmpty(analysis.TemperatureInstanceId))
            {
                analysisToSendDict[analysis.TemperatureInstanceId] = EEventType.Temperature;
            }
            if (!string.IsNullOrEmpty(analysis.SentinelStatusInstanceId))
            {
                analysisToSendDict[analysis.SentinelStatusInstanceId] = EEventType.SentinelStatus;
            }

            List<Task> tasksToRun = new List<Task>();
            foreach (KeyValuePair<string, EEventType> kvp in  analysisToSendDict)
            {
                string instanceId = kvp.Key;
                EEventType type = kvp.Value;
                AnalysisResult result = new AnalysisResult()
                {
                    AnalysisId = analysisId,
                    EventType = type,
                    InstanceId = instanceId,
                };
                tasksToRun.Add(_client.RaiseAnalysisResult(result));
            }
            await Task.WhenAll(tasksToRun);
            await _dbContextWrite.AnalysisUpdate(analysisId);
            return Ok();
        }
        #endregion

        #region Private
        private Dictionary<EEventType, object> GetAnalysisDataDicts(int analysisId, 
            Dictionary<int, SoilAnalysis?> soilsDict,
            Dictionary<int, MoistureAnalysis?> moisturesDict,
            Dictionary<int, TemperatureAnalysis?> temperaturesDict,
            Dictionary<int, SentinelStatusAnalysis?> statusesDict)
        {
            
            Dictionary<EEventType, object> dicts = new Dictionary<EEventType, object>();
            if (statusesDict.TryGetValue(analysisId, out SentinelStatusAnalysis? status) &&
                status != null)
            {
                dicts[EEventType.SentinelStatus] = status;
            }

            if (soilsDict.TryGetValue(analysisId, out SoilAnalysis? soil) &&
                soil != null)
            {
                dicts[EEventType.Soil] = soil;
            }

            if (moisturesDict.TryGetValue(analysisId, out MoistureAnalysis? moisture) &&
                moisture != null)
            {
                dicts[EEventType.Moisture] = moisture;
            }

            if (temperaturesDict.TryGetValue(analysisId , out TemperatureAnalysis? temperature) &&
                temperature != null)
            {
                dicts[EEventType.Temperature] = temperature;
            }           

            return dicts;
        }
        #endregion
    }
}
