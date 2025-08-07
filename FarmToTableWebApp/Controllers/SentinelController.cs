using Microsoft.AspNetCore.Mvc;

namespace FarmToTableWebApp.Controllers
{
    [Route("Sentinel")]
    public class SentinelController : Controller
    {
        #region Fields
        private readonly ILogger<SentinelController> _logger;
        #endregion

        #region Constructor
        public SentinelController(ILogger<SentinelController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region GET
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        #endregion
    }
}
