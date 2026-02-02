namespace Kinetq.LiquidMiddleware.Interfaces;

public interface ILiquidStartup
{
    Task RegisterRoutes();
    Task RegisterFilters();
}