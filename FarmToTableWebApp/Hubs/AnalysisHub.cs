using FarmToTableData.Models;
using Microsoft.AspNetCore.SignalR;

namespace FarmToTableWebApp.Hubs
{
    public class AnalysisHub : Hub
    {
        public async Task SendPendingAnalysis(EEventType type, object analysisObject)
        {
            await Clients.All.SendAsync("ReceiveNewPendingAnalysis", type, analysisObject);
        }
    }
}
