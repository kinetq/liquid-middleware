using Fluid;

namespace Kinetq.LiquidMiddleware.Models
{
    public class LiquidFilter
    {
        public string Name { get; set; } = null!;
        public FilterDelegate FilterDelegate { get; set; } = null!;
    }
}
