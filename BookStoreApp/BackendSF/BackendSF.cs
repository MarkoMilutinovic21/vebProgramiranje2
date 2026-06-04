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
                                services.AddCors(options =>
                                {
                                    options.AddPolicy("AllowReactApp", policy =>
                                    {
                                        policy.WithOrigins("http://localhost:5173")
                                              .AllowAnyHeader()
                                              .AllowAnyMethod();
                                    });
                                });
                            })
                            .Configure(app =>
                            {
                                app.UseCors("AllowReactApp");
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