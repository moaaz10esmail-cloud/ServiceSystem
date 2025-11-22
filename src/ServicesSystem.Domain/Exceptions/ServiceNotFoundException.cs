namespace ServicesSystem.Domain.Exceptions;

public class ServiceNotFoundException : DomainException
{
    public ServiceNotFoundException() 
        : base("Service not found") { }

    public ServiceNotFoundException(Guid serviceId) 
        : base($"Service with ID '{serviceId}' was not found") { }
}
