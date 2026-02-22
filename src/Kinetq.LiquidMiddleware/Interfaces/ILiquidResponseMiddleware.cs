using Kinetq.LiquidMiddleware.Models;

namespace Kinetq.LiquidMiddleware.Interfaces;

public interface ILiquidResponseMiddleware
{
    Task<LiquidResponseModel> HandleRequestAsync(LiquidRequestModel request);
}