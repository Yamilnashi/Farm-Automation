using FarmToTableData.Models;
using Newtonsoft.Json.Linq;
using System;

namespace FarmToTableData.Extensions
{
    public static class EventTypeExtensions
    {
        public static string EventName(this EEventType type)
        {
            string name;
            switch (type)
            {
                case EEventType.Temperature:
                case EEventType.Soil:
                case EEventType.Moisture:
                case EEventType.Pest:
                    name = type.ToString();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return $"{name}Approval";
        }       
    }
}
