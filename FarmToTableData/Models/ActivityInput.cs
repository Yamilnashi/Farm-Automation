using Newtonsoft.Json.Linq;

namespace FarmToTableData.Models
{
    public class ActivityInput
    {
        public EEventType EventType { get; set; }
        public JObject Change { get; set; }
        public string InstanceId { get; set; }
    }
}
