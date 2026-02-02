using Fluid;

namespace Kinetq.LiquidMiddleware.Interfaces;

public interface ILiquidFilter
{
    Task<(string, FilterDelegate)> GetFilter();
}