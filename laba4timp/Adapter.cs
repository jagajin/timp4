using System;

public interface ILogSink { void Write(LogEntry entry); }
// API
public class SyslogClient
{
    public void Send(int priority, string message)
    {
        Console.WriteLine($"SYSLOG priority={priority}: {message}"); // adapter stack
    }
}
public class SyslogAdapter : ILogSink
{
    private readonly SyslogClient _client;
    private readonly int _priority;
    public SyslogAdapter(SyslogClient client, int priority)
    { _client = client; _priority = priority; }
    public void Write(LogEntry entry)
    {
        string message = entry.GetRecord();
        _client.Send(_priority, message);
    }
}
public class LogService
{
    private readonly ILogSink _sink;
    public LogService(ILogSink sink) { _sink = sink; }
    public void Log(LogEntry entry) { _sink.Write(entry); }
}
