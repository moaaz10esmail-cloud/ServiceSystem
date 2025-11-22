namespace ServicesSystem.Domain.Enums;

public enum RequestStatus
{
    Pending = 1,        // في الانتظار
    Accepted = 2,       // مقبول
    InProgress = 3,     // قيد التنفيذ
    Completed = 4,      // مكتمل
    Cancelled = 5,      // ملغى
    Rejected = 6        // مرفوض
}
