using System.Xml.Linq;

public abstract class LogEntryDecorator : LogEntry
{
    protected readonly LogEntry _wrappee;
    protected LogEntryDecorator(LogEntry wrappee)
        : base(wrappee.CreatedAt, wrappee.GetMessage()) { _wrappee = wrappee; }
    public override string GetMessage() => _wrappee.GetMessage();
    public override string GetRecord() => _wrappee.GetRecord();
}
public class SeverityDecorator : LogEntryDecorator
{
    public string Severity { get; }
    public SeverityDecorator(LogEntry wrappee, string severity) : base(wrappee)
    { Severity = severity; }
    public override string GetRecord()
    { return $"[важность: {Severity}] " + _wrappee.GetRecord(); }
}
public class SourceDecorator : LogEntryDecorator
{
    public string Source { get; }
    public SourceDecorator(LogEntry wrappee, string source) : base(wrappee)
    { Source = source; }
    public override string GetRecord()
    { return $"[источник: {Source}] " + _wrappee.GetRecord(); }
}
public class XmlFormatDecorator : LogEntryDecorator
{
    public XmlFormatDecorator(LogEntry wrappee) : base(wrappee) { }
    public override string GetRecord()
    {
        string content = _wrappee.GetRecord();
        return new XElement("log", content).ToString(SaveOptions.DisableFormatting);
    }
}
