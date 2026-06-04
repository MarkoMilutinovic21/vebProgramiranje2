using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace BackendSF
{
    internal sealed class BackendSF : StatelessService
    {
        public BackendSF(StatelessServiceContext context)
            : base(context) { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        return new WebHostBuilder()
                            .UseKestrel()
                            .ConfigureServices(services =>
                            {
                                services.AddSingleton<StatelessServiceContext>(serviceContext);
                                services.AddControllers();
                            })
                            .Configure(app =>
                            {
                                app.UseRouting();
                                app.UseEndpoints(endpoints => endpoints.MapControllers());
                            })
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                            .UseUrls(url)
                            .Build();
                    }))
            };
        }
    }
}