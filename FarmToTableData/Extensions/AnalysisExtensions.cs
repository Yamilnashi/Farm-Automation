using FarmToTableData.Models;
using System;

namespace FarmToTableData.Extensions
{
    public static class AnalysisExtensions
    {
        public static object GetAnalysisChangeType(this Analysis a)
        {
            if (a.SoilAnalysisId != null)
            {
                return new SoilReadingHistoryChange()
                {
                    NPpm = (int)a.NPpm,
                    PPpm = (int)a.PPpm,
                    KPpm = (int)a.KPpm,
                    SavedDate = a.SavedDate,
                    SentinelId = a.SentinelId,
                };
            } else if (a.TemperatureAnalysisId != null)
            {
                return new TemperatureReadingHistoryChange()
                {
                    SavedDate = a.SavedDate,
                    SentinelId = a.SentinelId,
                    TemperatureCelsius = (decimal)a.TemperatureCelsius
                };
            } else if (a.MoistureAnalysisId != null)
            {
                return new MoistureReadingHistoryChange()
                {
                    SavedDate = a.SavedDate,
                    SentinelId = a.SentinelId,
                    Moisture = (byte)a.Moisture
                };
            } else if (a.SentinelStatusAnalysisId != null)
            {
                return new SentinelStatusHistoryChange()
                {
                    SavedDate = a.SavedDate,
                    SentinelId = a.SentinelId,
                    SentinelStatusCode = (ESentinelStatus)a.SentinelStatusCode
                };
            } else
            {
                throw new NotImplementedException();
            }                
        }
    }
}
