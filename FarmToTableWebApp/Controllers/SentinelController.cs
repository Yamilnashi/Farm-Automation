using FarmToTableData.Interfaces;
using FarmToTableData.Models;
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
                IEnumerable<AnalysisTableItemViewModel> models = analysis
                    .Select(x => new AnalysisTableItemViewModel()
                    {
                        AnalysisId = x.AnalysisId,
                        InstanceId = x.InstanceId,
                        SentinelId = x.SentinelId,
                        IsAnalyzed = x.IsAnalyzed,
                        SavedDate = x.SavedDate,
                    });
                json = JsonConvert.SerializeObject(new { data = models });
            } catch (Exception ex)
            {
                _logger.LogError($"An error occurred trying to get analysis data for table: {ex.Message}");
            }
            return Content(json, "application/json");
        }

        #endregion
    }
}
