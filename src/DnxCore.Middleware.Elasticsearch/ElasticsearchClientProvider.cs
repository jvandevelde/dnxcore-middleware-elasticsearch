using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Diagnostics;
using System.Text;

namespace JV.DnxCore.Middleware.Elasticsearch
{
    public class ElasticsearchClientProvider
    {
        private static IConfiguration _configuration;
        private static ElasticClient _esClient;
        
        static ElasticsearchClientProvider()
        {
            // Set up configuration sources.
            var _configBuilder = new ConfigurationBuilder()
                .AddJsonFile("config.json");

            _configuration = _configBuilder.Build();
        }

        public static ElasticClient Instance
        {
            get
            {
                if (_esClient == null)
                    _esClient = BuildElasticClientInstance();

                return _esClient;
            }
        }

        private static ElasticClient BuildElasticClientInstance()
        {
            var esHost1Address = _configuration["AppConfiguration:Elasticsearch:Server1Addresss"];
            var esHost1Port = _configuration["AppConfiguration:Elasticsearch:Server1Port"];
            var esHost2Address = _configuration["AppConfiguration:Elasticsearch:Server2Addresss"];
            var esHost2Port = _configuration["AppConfiguration:Elasticsearch:Server2Port"];
            var esHost3Address = _configuration["AppConfiguration:Elasticsearch:Server3Addresss"];
            var esHost3Port = _configuration["AppConfiguration:Elasticsearch:Server3Port"];
            
            var esNode1 = new Uri(string.Format("http://{0}:{1}", esHost1Address, esHost1Port));
            var esNode2 = new Uri(string.Format("http://{0}:{1}", esHost2Address, esHost2Port));
            var esNode3 = new Uri(string.Format("http://{0}:{1}", esHost3Address, esHost3Port));

            var connectionPool = new SniffingConnectionPool(new[] { esNode1, esNode2, esNode3 });

            var settings = new ConnectionSettings(connectionPool)
                .DefaultIndex("middleware.request.logging")
                //.EnableTrace(true).ExposeRawResponse(true);   // Not currently supported in new 2.0 client. Use OnRequestCompleted() instead
                .DisableDirectStreaming()
                .OnRequestCompleted(details =>
                {
                    Debug.WriteLine("### ES REQEUST ###");
                    if (details.RequestBodyInBytes != null) Debug.WriteLine(Encoding.UTF8.GetString(details.RequestBodyInBytes));
                    Debug.WriteLine("### ES RESPONSE ###");
                    if (details.ResponseBodyInBytes != null) Debug.WriteLine(Encoding.UTF8.GetString(details.ResponseBodyInBytes));
                })
                .PrettyJson();

            return new ElasticClient(settings);
        }
    }
}

