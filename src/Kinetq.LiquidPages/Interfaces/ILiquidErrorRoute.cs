using Kinetq.LiquidPages.Models;

namespace Kinetq.LiquidPages.Interfaces;

public interface ILiquidErrorRoute
{
    Task<LiquidRoute> GetRoute();
    int StatusCode { get; }
}