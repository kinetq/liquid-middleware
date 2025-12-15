namespace Kinetq.LiquidSimpleServer.Exceptions;

public class LiquidSyntaxException : Exception
{
    public LiquidSyntaxException(string message) : base(message)
    {
    }
}