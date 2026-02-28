using Kinetq.LiquidPages.Interfaces;
using Kinetq.LiquidPages.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Kinetq.LiquidPages.Helpers;

public static class ServiceCollectionHelpers
{
    public static IServiceCollection AddLiquidPages(
        this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ILiquidFilterManager, LiquidFilterManager>();
        serviceCollection.AddSingleton<ILiquidRegisteredTypesManager, LiquidRegisteredTypesManager>();
        serviceCollection.AddSingleton<ILiquidRoutesManager, LiquidRoutesManager>();
        serviceCollection.AddSingleton<IFluidParserManager, FluidParserManager>();
        serviceCollection.AddScoped<IHtmlRenderer, HtmlRenderer>();
        serviceCollection.AddScoped<ILiquidResponseMiddleware, LiquidResponseMiddleware>();
        serviceCollection.AddScoped<ILiquidStartup, LiquidStartup>();

        return serviceCollection;
    }
}