using Microsoft.Extensions.Hosting;
using Serilog;

namespace Common.Logging
{
    public static class SeriLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
           (context, configuration) =>
           {
               //var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");

               configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithCorrelationId()
                    .Enrich.WithClientIp()
                    .Enrich.WithClientAgent()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.WithAssemblyVersion()
                    .Enrich.WithAssemblyInformationalVersion()
                    .Enrich.WithThreadId()
                    .Enrich.WithThreadName()
                    .Enrich.WithUserName()
                    .WriteTo.Debug()
                    .WriteTo.Console()
                    //.WriteTo.Elasticsearch(
                    //    new ElasticsearchSinkOptions(new Uri(elasticUri))
                    //    {
                    //        IndexFormat = $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                    //        AutoRegisterTemplate = true,
                    //        NumberOfShards = 2,
                    //        NumberOfReplicas = 1
                    //    })
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .ReadFrom.Configuration(context.Configuration);
           };
    }
}