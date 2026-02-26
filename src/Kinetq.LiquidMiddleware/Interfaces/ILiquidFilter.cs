using Kinetq.LiquidMiddleware.Models;

namespace Kinetq.LiquidMiddleware.Interfaces;

public interface ILiquidFilter
{
    Task<LiquidFilter> GetFilter();
}