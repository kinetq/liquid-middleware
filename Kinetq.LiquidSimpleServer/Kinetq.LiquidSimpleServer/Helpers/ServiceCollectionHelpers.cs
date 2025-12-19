using Kinetq.LiquidSimpleServer.Interfaces;
using Kinetq.LiquidSimpleServer.Managers;
using Kinetq.LiquidSimpleServer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kinetq.LiquidSimpleServer.Helpers;

public static class ServiceCollectionHelpers
{
    public static IServiceCollection AddLiquidSimpleServer(
        this IServiceCollection serviceCollection, 
        IConfiguration? configuration = null)
    {
        serviceCollection.AddSingleton<ILiquidSimpleServer, LiquidSimpleServer>();
        serviceCollection.AddSingleton<ILiquidFilterManager, LiquidFilterManager>();
        serviceCollection.AddSingleton<ILiquidRegisteredTypesManager, LiquidRegisteredTypesManager>();
        serviceCollection.AddSingleton<ILiquidRoutesManager, LiquidRoutesManager>();
        serviceCollection.AddSingleton<IFluidParserManager, FluidParserManager>();
        serviceCollection.AddScoped<IHtmlRenderer, HtmlRenderer>();

        if (configuration != null)
        {
            serviceCollection.Configure<LiquidSimpleServerOptions>(configuration.GetSection("LiquidSimpleServer").Bind);
        }
        else
        {
            serviceCollection.Configure<LiquidSimpleServerOptions>(options => new LiquidSimpleServerOptions());
        }

        return serviceCollection;
    }
}