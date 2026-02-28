namespace Kinetq.LiquidPages.Models;

public class LiquidResponseModel
{
    public string ContentType { get; set; }
    public byte[] Content { get; set; }
    public int StatusCode { get; set; }
}