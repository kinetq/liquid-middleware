using System.Text.RegularExpressions;
using Microsoft.Extensions.FileProviders;

namespace Kinetq.LiquidMiddleware.Models;

public partial class LiquidRoute
{
    public virtual Regex RoutePattern { get; set; }
    public virtual string LiquidTemplatePath { get; set; }
    public virtual IFileProvider FileProvider { get; set; }
    public virtual Func<LiquidRequestModel, Task<object>> Execute { get; set; }
    public virtual IDictionary<string, string> QueryParams { get; set; } = new Dictionary<string, string>();
}