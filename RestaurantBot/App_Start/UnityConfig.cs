using Microsoft.Practices.Unity;
using Microsoft.WindowsAzure.Storage;
using RestaurantBot.Utils;
using System.Configuration;
using System.Web.Http;
using Unity.WebApi;

namespace RestaurantBot
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            var container = InjectionContainer.Instance.Container;
            // e.g. container.RegisterType<ITestService, TestService>();
            RegisterTypes(container);
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
        private static void RegisterTypes(UnityContainer container)
        {
                      // Singleton Azure blob storage provider
            var azureStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            var azureBlobProvider = new AzureStorageProvider(azureStorageAccount);

            // Telemetry Client
           // var telemetryClient = new TelemetryClient(TelemetryConfiguration.Active);

            container.RegisterInstance<AzureStorageProvider>(azureBlobProvider, new ContainerControlledLifetimeManager());

            //Logger.LogInfo("UnityConfig: Feedback bot successfully initialized.");
        }
    }
}