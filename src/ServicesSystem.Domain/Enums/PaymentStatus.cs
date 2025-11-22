namespace ServicesSystem.Domain.Enums;

public enum PaymentStatus
{
    Pending = 1,        // في الانتظار
    Completed = 2,      // مكتمل
    Failed = 3,         // فشل
    Refunded = 4        // مسترجع
}
