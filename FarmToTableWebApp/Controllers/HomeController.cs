using Microsoft.AspNetCore.Mvc;

namespace FarmToTableWebApp.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        #region Fields
        private readonly ILogger<HomeController> _logger;
        #endregion

        #region Constructor
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region GET
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
        #endregion
    }
}
