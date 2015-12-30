using TypeLite;

namespace JobTimer.WebApplication.TypeScript
{    
    public class HttpHeaders
    {
        [TsClass(Module = "Constants.HttpHeaders")]
        public class Request
        {
            public const string SignalRConnectionId = "SIGNALR_CONN_ID";
            public const string CurrentTimeZone = "TIMEZONE";
        }
        public static class Response
        {
        }
    }
}
