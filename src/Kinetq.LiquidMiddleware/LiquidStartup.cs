using Kinetq.LiquidMiddleware.Interfaces;

namespace Kinetq.LiquidMiddleware;

public class LiquidStartup : ILiquidStartup
{
    private readonly ILiquidRoutesManager _liquidRoutesManager;
    private readonly ILiquidFilterManager _liquidFilterManager;
    private readonly IEnumerable<ILiquidRoute> _liquidRoutes;
    private readonly IEnumerable<ILiquidFilter> _liquidFilters;
    private readonly IEnumerable<ILiquidErrorRoute> _liquidErrorRoutes;

    public LiquidStartup(
        ILiquidRoutesManager liquidRoutesManager,
        IEnumerable<ILiquidRoute> liquidRoutes,
        IEnumerable<ILiquidFilter> liquidFilters,
        ILiquidFilterManager liquidFilterManager, 
        IEnumerable<ILiquidErrorRoute> liquidErrorRoutes)
    {
        _liquidRoutesManager = liquidRoutesManager;
        _liquidRoutes = liquidRoutes;
        _liquidFilters = liquidFilters;
        _liquidFilterManager = liquidFilterManager;
        _liquidErrorRoutes = liquidErrorRoutes;
    }

    public async Task RegisterRoutes()
    {
        foreach (var route in _liquidRoutes)
        {
            _liquidRoutesManager.RegisterRoute(await route.GetRoute());
        }

        foreach (var liquidErrorRoute in _liquidErrorRoutes)
        {
            var route = await liquidErrorRoute.GetRoute();
            _liquidRoutesManager.RegisterErrorRoute(liquidErrorRoute.StatusCode, route);
        }
    }

    public async Task RegisterFilters()
    {
        foreach (var liquidFilter in _liquidFilters)
        {
            var filter = await liquidFilter.GetFilter();
            _liquidFilterManager.RegisterFilter(filter.Name, filter.FilterDelegate);
        }
    }
}