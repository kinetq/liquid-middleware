using Kinetq.LiquidMiddleware.Models;

namespace Kinetq.LiquidMiddleware.Interfaces;

public interface ILiquidRoute
{
    Task<LiquidRoute> GetRoute();
}