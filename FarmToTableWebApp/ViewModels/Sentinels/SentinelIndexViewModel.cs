using FarmToTableWebApp.Data;

namespace FarmToTableWebApp.ViewModels.Sentinels
{
    public class SentinelIndexViewModel
    {
        public string ColDefs_SentinelsPendingAnalysis
        {
            get
            {
                return new Mapper().GetColumnsJson(typeof(AnalysisTableItemViewModel));
            }
        }
    }
}
