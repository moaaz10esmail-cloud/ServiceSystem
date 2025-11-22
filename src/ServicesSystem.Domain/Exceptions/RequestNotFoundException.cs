namespace ServicesSystem.Domain.Exceptions;

public class RequestNotFoundException : DomainException
{
    public RequestNotFoundException() 
        : base("Service request not found") { }

    public RequestNotFoundException(Guid requestId) 
        : base($"Service request with ID '{requestId}' was not found") { }
}
