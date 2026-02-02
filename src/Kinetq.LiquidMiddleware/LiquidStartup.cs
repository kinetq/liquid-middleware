using Kinetq.LiquidMiddleware.Interfaces;

namespace Kinetq.LiquidMiddleware;

public class LiquidStartup : ILiquidStartup
{
    private readonly ILiquidRoutesManager _liquidRoutesManager;
    private readonly ILiquidFilterManager _liquidFilterManager;
    private readonly IEnumerable<ILiquidRoute> _liquidRoutes;
    private readonly IEnumerable<ILiquidFilter> _liquidFilters;

    public LiquidStartup(
        ILiquidRoutesManager liquidRoutesManager,
        IEnumerable<ILiquidRoute> liquidRoutes,
        IEnumerable<ILiquidFilter> liquidFilters,
        ILiquidFilterManager liquidFilterManager)
    {
        _liquidRoutesManager = liquidRoutesManager;
        _liquidRoutes = liquidRoutes;
        _liquidFilters = liquidFilters;
        _liquidFilterManager = liquidFilterManager;
    }

    public async Task RegisterRoutes()
    {
        foreach (var route in _liquidRoutes)
        {
            _liquidRoutesManager.RegisterRoute(await route.GetRoute());
        }
    }

    public async Task RegisterFilters()
    {
        foreach (var liquidFilter in _liquidFilters)
        {
            var filter = await liquidFilter.GetFilter();
            _liquidFilterManager.RegisterFilter(filter.Item1, filter.Item2);
        }
    }
}