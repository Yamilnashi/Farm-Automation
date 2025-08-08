using FarmToTableData.Models;
using Newtonsoft.Json.Linq;
using System;

namespace FarmToTableData.Utils
{
    public static class CdcHelper
    {
        public static MoistureReadingHistoryChange GetMoistureReadingHistoryChange(JObject obj)
        {

            T TryGetAndSet<T>(string propertyName)
            {
                if (obj.TryGetValue(propertyName, out JToken value) &&
                    value != null)
                {
                    return value.ToObject<T>();
                }
                return default(T);
            }

            MoistureReadingHistoryChange model = new MoistureReadingHistoryChange();
            model.MoistureReadingHistoryId = TryGetAndSet<int>(nameof(model.MoistureReadingHistoryId));
            model.Moisture = TryGetAndSet<byte>(nameof(model.Moisture));
            model.Operation = TryGetAndSet<ECdcChangeType>(nameof(model.Operation));
            model.SentinelId = TryGetAndSet<int>(nameof(model.SentinelId));
            model.SavedDate = TryGetAndSet<DateTime>(nameof(model.SavedDate));
            return model;
        }
    }
}
