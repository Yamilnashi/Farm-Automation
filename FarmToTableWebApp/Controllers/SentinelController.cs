using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using FarmToTableWebApp.ViewModels.Sentinels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FarmToTableData.Extensions;

namespace FarmToTableWebApp.Controllers
{
    [Route("Sentinel")]
    public class SentinelController : Controller
    {
        #region Fields
        private readonly ILogger<SentinelController> _logger;
        private readonly IDboDbContextRead _dbContextRead;
        private readonly IDboDbContextWrite _dbContextWrite;
        #endregion

        #region Constructor
        public SentinelController(ILogger<SentinelController> logger,
            IDboDbContextRead dbContextRead,
            IDboDbContextWrite dbContextWrite)
        {
            _logger = logger;
            _dbContextRead = dbContextRead;
            _dbContextWrite = dbContextWrite;
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
        public async Task<IActionResult> AnalysisTableJson()
        {
            string json = string.Empty;
            try
            {
                IEnumerable<Analysis> analysis = await _dbContextRead.AnalysisList();

                Dictionary<int, Analysis> analysisDict = analysis
                    .GroupBy(x => x.SentinelId)
                    .ToDictionary(x => x.Key, x => x.First());
                Dictionary<int, Dictionary<string, object>> analysisDataDicts = analysis
                    .GroupBy(x => x.SentinelId)
                    .ToDictionary(x => x.Key, x => GetAnalysisDataDicts(x.ToArray()));

                IEnumerable<AnalysisTableItemViewModel> models = analysisDict.Keys
                    .Select(sentinelId => new AnalysisTableItemViewModel()
                    {
                        AnalysisId = analysisDict[sentinelId].AnalysisId,
                        InstanceId = analysisDict[sentinelId].InstanceId,
                        SentinelId = sentinelId,
                        IsAnalyzed = analysisDict[sentinelId].IsAnalyzed,
                        SavedDate = analysisDict[sentinelId].SavedDate,
                        AnalysisDataDict = analysisDataDicts[sentinelId]
                    });
                json = JsonConvert.SerializeObject(new { data = models });
            } catch (Exception ex)
            {
                _logger.LogError($"An error occurred trying to get analysis data for table: {ex.Message}");
            }
            return Content(json, "application/json");
        }

        #endregion

        #region Private
        private Dictionary<string, object> GetAnalysisDataDicts(Analysis[] analyses)
        {
            Dictionary<string, object> dicts = new Dictionary<string, object>();            
            for (int i = 0; i < analyses.Length; i++)
            {
                Analysis a = analyses[i];
                object obj = a.GetAnalysisChangeType();
                dicts.TryAdd(a.InstanceId, obj);
            }
            return dicts;
        }
        #endregion
    }
}
