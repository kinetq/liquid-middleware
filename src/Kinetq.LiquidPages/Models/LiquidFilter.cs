using Fluid;

namespace Kinetq.LiquidPages.Models
{
    public class LiquidFilter
    {
        public string Name { get; set; } = null!;
        public FilterDelegate FilterDelegate { get; set; } = null!;
    }
}
