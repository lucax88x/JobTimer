using System;
using System.Data.SqlTypes;
using AutoMapper;

namespace JobTimer.Utils
{
    public interface ITimeGetter
    {
        DateTime Now { get; }
    }

    public class TimeGetter : ITimeGetter
    {
        public DateTime Now => DateTime.UtcNow;        
    }
}
