using Newtonsoft.Json;

namespace FarmToTableWebApp.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataTableColumnAttribute : Attribute
    {
        [JsonProperty("targets", NullValueHandling = NullValueHandling.Ignore)]
        public int[]? ColIndexs { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string? ColName { get; private set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }

        [JsonProperty("searchable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Searchable { get; private set; }

        [JsonProperty("orderable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Orderable { get; private set; }

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string? ColType { get; private set; }

        [JsonProperty(PropertyName = "width", NullValueHandling = NullValueHandling.Ignore)]
        public string? Width { get; private set; }

        [JsonProperty(PropertyName = "className", NullValueHandling = NullValueHandling.Ignore)]
        public string? ClassName { get; private set; }

        [JsonProperty(PropertyName = "defaultContent")]
        public string? DefaultContent { get; set; }

        [JsonIgnore]
        public override object TypeId => base.TypeId;

        public DataTableColumnAttribute()
        {
        }

        public DataTableColumnAttribute(int[] colIndex, string? colName = "", bool searchable = true, bool orderable = true, string? colType = null, string? width = null, string? className = null, string defaultContent = "")
        {
            ColIndexs = colIndex;
            ColName = colName;
            Searchable = searchable;
            Orderable = orderable;
            ColType = colType;
            Width = width;
            ClassName = className;
            DefaultContent = defaultContent;
        }

        public DataTableColumnAttribute(int colIndex, string? colName = null, bool searchable = true, bool orderable = true, string? colType = null, string? width = null, string? className = null, string defaultContent = "")
            : this([colIndex], colName, searchable, orderable, colType, width, className, defaultContent)
        {
        }
    }
}
