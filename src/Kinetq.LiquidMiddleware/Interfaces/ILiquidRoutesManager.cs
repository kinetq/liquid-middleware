using Kinetq.LiquidMiddleware.Models;
using Microsoft.Extensions.FileProviders;
using System.Net;

namespace Kinetq.LiquidMiddleware.Interfaces;

public interface ILiquidRoutesManager
{
    void RegisterRoute(LiquidRoute route);
    void RegisterErrorRoute(int statusCode, LiquidRoute route);
    IList<LiquidRoute> LiquidRoutes { get; }
    IDictionary<int, LiquidRoute> ErrorRoutes { get; }
    LiquidRoute? GetRouteForPath(string path, IDictionary<string, string>? queryParams = null);
    LiquidRoute? GetRouteForStatusCode(HttpStatusCode statusCode);
    IFileProvider? GetFileProviderForAsset(string filePath);
}