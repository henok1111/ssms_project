namespace SsmsApi.Domain.Enums;

public enum JobStatus
{
    Open,           // posted, accepting applications
    Assigned,       // worker accepted, quote pending/approved
    InProgress,     // work has started
    Completed,      // worker marked done
    Closed,         // client confirmed + paid
    Cancelled
}