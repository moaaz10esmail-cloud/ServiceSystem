namespace ServicesSystem.Domain.Exceptions;

public class InsufficientBalanceException : DomainException
{
    public InsufficientBalanceException() 
        : base("Insufficient balance") { }

    public InsufficientBalanceException(decimal required, decimal available) 
        : base($"Insufficient balance. Required: {required:C}, Available: {available:C}") { }
}
