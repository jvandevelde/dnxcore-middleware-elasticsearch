using Microsoft.AspNet.Builder;

namespace JV.DnxCore.Middleware.Elasticsearch
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseElasticsearchRequestLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ElasticsearchRequestLoggingMiddleware>();
        }
    }
}
