using JV.DnxCore.Middleware.Elasticsearch.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace JV.DnxCore.Middleware.Elasticsearch
{
    public class ElasticsearchRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ElasticsearchRequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var loggingDoc = new RequestLogDocument
            {
                RequestTime = DateTime.Now,
                Path = context.Request.Path
            };

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var esClient = ElasticsearchClientProvider.Instance;

            await _next(context);

            stopWatch.Stop();
            loggingDoc.ProcessingTime = stopWatch.ElapsedMilliseconds;

            await esClient.IndexAsync(loggingDoc);
        }
    }
}
