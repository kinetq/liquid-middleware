using Kinetq.LiquidPages.Models;

namespace Kinetq.LiquidPages.Interfaces;

public interface ILiquidRoute
{
    Task<LiquidRoute> GetRoute();
}