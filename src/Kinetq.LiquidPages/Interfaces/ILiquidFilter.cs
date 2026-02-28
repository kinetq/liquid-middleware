using Kinetq.LiquidPages.Models;

namespace Kinetq.LiquidPages.Interfaces;

public interface ILiquidFilter
{
    Task<LiquidFilter> GetFilter();
}