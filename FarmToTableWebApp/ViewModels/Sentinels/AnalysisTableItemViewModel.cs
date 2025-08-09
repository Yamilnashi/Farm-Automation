using FarmToTableData.Interfaces;
using FarmToTableWebApp.Data;

namespace FarmToTableWebApp.ViewModels.Sentinels
{
    public class AnalysisTableItemViewModel
    {
        [DataTableColumn(0, "", orderable: false)]
        public int AnalysisId { get; set; }
        [DataTableColumn(1, "Sentinel ID")]
        public int SentinelId { get; set; }
        [DataTableColumn(2, "Orchestrator Instance ID")]
        public string InstanceId { get; set; } = string.Empty;
        public bool IsAnalyzed { get; set; }
        [DataTableColumn(3, "Saved Date")]
        public DateTime SavedDate { get; set; }
        [DataTableColumn(4, "Data")]
        public Dictionary<string, object> AnalysisDataDict { get; set; } = [];
    }
}
