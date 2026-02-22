using System.Net;
using System.Text;
using Kinetq.LiquidMiddleware.Helpers;
using Kinetq.LiquidMiddleware.Interfaces;
using Kinetq.LiquidMiddleware.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Kinetq.LiquidMiddleware;

public class LiquidResponseMiddleware : ILiquidResponseMiddleware
{
    private readonly ILiquidRoutesManager _liquidRoutesManager;
    private readonly IHtmlRenderer _htmlRenderer;
    private readonly ILogger<LiquidResponseMiddleware> _logger;

    public LiquidResponseMiddleware(
        ILiquidRoutesManager liquidRoutesManager,
        IHtmlRenderer htmlRenderer,
        ILogger<LiquidResponseMiddleware> logger)
    {
        _liquidRoutesManager = liquidRoutesManager;
        _htmlRenderer = htmlRenderer;
        _logger = logger;
    }

    public async Task<LiquidResponseModel> HandleRequestAsync(LiquidRequestModel request)
    {
        var renderModel = new RenderModel
        {
            Route = request.Route,
            QueryParams = request.QueryParams
        };

        LiquidRoute? liquidRoute = _liquidRoutesManager.GetRouteForPath(request.Route, renderModel.QueryParams);
        if (liquidRoute?.Execute != null)
        {
            try
            {

                renderModel.ViewModel = await liquidRoute.Execute(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing route logic for path {Path}", request.Route);
                var errorRoute = _liquidRoutesManager.GetRouteForStatusCode(HttpStatusCode.InternalServerError);
                var errorHtmlResponse = await _htmlRenderer.RenderHtml(renderModel, errorRoute);
                if (errorHtmlResponse != null)
                {
                    return new LiquidResponseModel 
                    { 
                        Content = Encoding.UTF8.GetBytes(errorHtmlResponse), 
                        ContentType = "text/html", 
                        StatusCode = (int)HttpStatusCode.InternalServerError 
                    };
                }

                return new LiquidResponseModel
                {
                    Content = Encoding.UTF8.GetBytes($"<h1>500 - Internal Server Error</h1><p>{ex.Message}</p>"),
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        // Handle static routes
        var htmlResponse = await _htmlRenderer.RenderHtml(renderModel, liquidRoute);
        if (htmlResponse != null)
        {
            return new LiquidResponseModel
            {
                Content = Encoding.UTF8.GetBytes(htmlResponse),
                ContentType = "text/html",
                StatusCode = 200
            };
        }

        string extension = Path.GetExtension(request.Route);
        string assetContentType = extension.GetContentType();
        if (!string.IsNullOrEmpty(assetContentType))
        {
            try
            {
                string referer = request.Headers["Referer"];
                IFileProvider? assetFileProvider = null;
                if (!string.IsNullOrEmpty(referer))
                {
                    Uri refererUri = new Uri(referer);
                    LiquidRoute? referrerLiquidRoute = _liquidRoutesManager.GetRouteForPath(refererUri.AbsolutePath);
                    assetFileProvider = referrerLiquidRoute?.FileProvider;
                }

                assetFileProvider ??= _liquidRoutesManager.GetFileProviderForAsset(request.Route);

                var fileInfo = assetFileProvider?.GetFileInfo(request.Route);
                if (fileInfo is { Exists: true })
                {
                    var fileContent = await fileInfo.GetFileContentsBytes();
                    var contentType = await fileInfo.GetFileContentType();
                    return new LiquidResponseModel
                    {
                        Content = fileContent,
                        ContentType = contentType,
                        StatusCode = 200
                    };
                }
            }
            catch
            {
                // Fall through to 404
            }
        }

        var notFoundRoute = _liquidRoutesManager.GetRouteForStatusCode(HttpStatusCode.NotFound);
        var notFoundHtmlResponse = await _htmlRenderer.RenderHtml(renderModel, notFoundRoute);
        if (notFoundHtmlResponse != null)
        {
            return new LiquidResponseModel
            {
                Content = Encoding.UTF8.GetBytes(notFoundHtmlResponse),
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }

        return new LiquidResponseModel
        {
            Content = Encoding.UTF8.GetBytes("<h1>404 - Page Not Found</h1>"),
            ContentType = "text/html",
            StatusCode = 404
        };
    }
}