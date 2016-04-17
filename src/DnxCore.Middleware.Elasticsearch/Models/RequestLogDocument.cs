using System;

namespace JV.DnxCore.Middleware.Elasticsearch.Models
{
    public class RequestLogDocument
    {
        public DateTime RequestTime { get; set; }
        public string Path { get; set; }
        public long ProcessingTime { get; set; }
    }
}
