using Kinetq.LiquidMiddleware.Models;

namespace Kinetq.LiquidMiddleware.Interfaces;

public interface ILiquidErrorRoute
{
    Task<LiquidRoute> GetRoute();
    int StatusCode { get; }
}