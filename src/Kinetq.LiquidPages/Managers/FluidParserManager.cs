using Fluid;
using Kinetq.LiquidPages.Interfaces;

namespace Kinetq.LiquidPages.Managers;

public class FluidParserManager : IFluidParserManager
{
    private readonly Lazy<FluidParser> _fluidParsers = new(() => new FluidParser());
    public FluidParser FluidParser => _fluidParsers.Value;
}