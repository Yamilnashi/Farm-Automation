using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using FarmToTableWebApp.Data;

namespace FarmToTableWebApp.ViewModels.Sentinels
{
    public class AnalysisTableItemViewModel
    {
        [DataTableColumn(0, "", orderable: false)]
        public int AnalysisId { get; set; }
        [DataTableColumn(1, "Sentinel ID")]
        public int SentinelId { get; set; }
        public bool IsAnalyzed { get; set; }
        [DataTableColumn(2, "Saved Date")]
        public DateTime SavedDate { get; set; }
        [DataTableColumn(3, "Data")]
        public Dictionary<EEventType, object> AnalysisDataDict { get; set; } = [];
    }
}
