using System;

public abstract class LogEntry
{
    public DateTime CreatedAt { get; protected set; }
    protected string _message;
    protected LogEntry(DateTime createdAt, string message)
    { CreatedAt = createdAt; _message = message; }
    public virtual string GetMessage() => _message;
    public virtual string GetRecord()
    { return $"[{CreatedAt:yyyy-MM-dd HH:mm:ss}] {_message}"; }
}
public class SystemLogEntry : LogEntry
{
    public string Component { get; }
    public SystemLogEntry(DateTime createdAt, string message, string component)
        : base(createdAt, message) { Component = component; }
    public override string GetRecord()
    { return base.GetRecord() + $" (компонент: {Component})"; }
}
public class SecurityLogEntry : LogEntry
{
    public string IncidentType { get; }
    public SecurityLogEntry(DateTime createdAt, string message, string incidentType)
        : base(createdAt, message) { IncidentType = incidentType; }
    public override string GetRecord()
    { return base.GetRecord() + $" (тип инцидента: {IncidentType})"; }
}
