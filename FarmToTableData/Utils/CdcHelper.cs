using FarmToTableData.Interfaces;
using FarmToTableData.Models;
using Newtonsoft.Json.Linq;
using System;

namespace FarmToTableData.Utils
{
    public sealed class CdcHelper
    {
        #region Fields
        private readonly JObject _instance = new JObject();
        private readonly ECdcChangeType _operation;
        private readonly int _sentinelId;
        private readonly DateTime _savedDate;
        #endregion

        #region Constructor
        public CdcHelper(JObject obj)
        {
            _instance = obj;
            _operation = TryGetValue<ECdcChangeType>(nameof(IChangeBase.Operation));
            _sentinelId = TryGetValue<int>(nameof(IChangeBase.SentinelId));
            _savedDate = TryGetValue<DateTime>(nameof(IChangeBase.SavedDate));
        }
        #endregion

        #region Methods
        public MoistureReadingHistoryChange GetMoistureReadingHistoryChange()
        {
            MoistureReadingHistoryChange model = new MoistureReadingHistoryChange()
            {
                MoistureReadingHistoryId = TryGetValue<int>(nameof(model.MoistureReadingHistoryId)),
                Moisture = TryGetValue<byte>(nameof(model.Moisture)),
                Operation = _operation,
                SentinelId = _sentinelId,
                SavedDate = _savedDate
            };
            return model;
        }
        public TemperatureReadingHistoryChange GetTemperatureReadingHistoryChange()
        {
            TemperatureReadingHistoryChange model = new TemperatureReadingHistoryChange()
            {
                TemperatureReadingHistoryId = TryGetValue<int>(nameof(model.TemperatureReadingHistoryId)),
                TemperatureCelsius = TryGetValue<decimal>(nameof(model.TemperatureCelsius)),
                Operation = _operation,
                SentinelId = _sentinelId,
                SavedDate = _savedDate
            };
            return model;
        }
        public SoilReadingHistoryChange GetSoilReadingHistoryChange()
        {
            SoilReadingHistoryChange model = new SoilReadingHistoryChange()
            {
                SoilReadingHistoryId = TryGetValue<int>(nameof(model.SoilReadingHistoryId)),
                NPpm = TryGetValue<int>(nameof(model.NPpm)),
                PPpm = TryGetValue<int>(nameof(model.PPpm)),
                KPpm = TryGetValue<int>(nameof(model.KPpm)),
                Operation = _operation,
                SentinelId = _sentinelId,
                SavedDate = _savedDate
            };
            return model;
        }
        public SentinelStatusHistoryChange GetSentinelStatusHistoryChange()
        {
            SentinelStatusHistoryChange model = new SentinelStatusHistoryChange()
            {
                SentinelStatusHistoryId = TryGetValue<int>(nameof(model.SentinelStatusHistoryId)),
                SentinelStatusCode = TryGetValue<ESentinelStatus>(nameof(model.SentinelStatusCode)),
                Operation = _operation,
                SentinelId = _sentinelId,
                SavedDate = _savedDate
            };
            return model;
        }
        #endregion

        #region Private
        private T TryGetValue<T>(string propertyName)
        {
            if (_instance.TryGetValue(propertyName, out JToken value) &&
                value != null)
            {
                return value.ToObject<T>();
            }
            return default(T);
        }
        #endregion
    }
}
