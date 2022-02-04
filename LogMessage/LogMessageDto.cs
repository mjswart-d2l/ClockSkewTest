namespace LogMessage;

public record LogMessageDto (
    int LogId,
    string Message,
    DateTime LastUpdatedTime
);