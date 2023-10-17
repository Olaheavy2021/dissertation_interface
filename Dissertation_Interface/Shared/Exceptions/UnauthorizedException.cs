namespace Shared.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base($"Unauthorized access to this action")
    {

    }
}