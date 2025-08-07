using System;

namespace FarmToTableData.Models
{
    public abstract class HistoryChangeBase : ChangeBase
    {        
        public abstract int HistoryId { get; }

    }
}
