namespace InquirySample.Domain.Entities;

public class ErrorLog
{
    public int Id { get; private set; }
    public string Source { get; private set; }
    public string Exception { get; private set; }
    public DateTime CreatedAt { get; set; }
    private ErrorLog(string source, string exception)
    {
        Source = source;
        Exception = exception;
        CreatedAt = DateTime.UtcNow;
    }

    public static ErrorLog Create(string source, string exception)
    {
        return new ErrorLog(source, exception);
    }
}
