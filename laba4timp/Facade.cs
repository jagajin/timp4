using System;
using System.Collections.Generic;

public class FilterService
{
    public bool Allow(LogEntry entry)
    { return !string.IsNullOrWhiteSpace(entry.GetMessage()); }
}
public class FormatterService
{
    public string Format(LogEntry entry) { return entry.GetRecord(); }
}
public class StorageService
{
    private readonly List<string> _records = new();
    public int Count => _records.Count;
    public void Save(string record)
    {
        _records.Add(record); // facade stack
    }
}
public class AlertService
{
    private readonly List<string> _alerts = new();
    public int Count => _alerts.Count;
    public void Notify(string record)
    { _alerts.Add(record); Console.WriteLine("Оповещение: " + record); }
}
public class LoggingFacade
{
    private readonly FilterService _filter = new();
    private readonly FormatterService _formatter = new();
    private readonly StorageService _storage = new();
    private readonly AlertService _alerts = new();
    public bool Log(LogEntry entry, bool notify)
    {
        if (!_filter.Allow(entry)) return false;
        string record = _formatter.Format(entry);
        _storage.Save(record);
        if (notify) _alerts.Notify(record);
        return true;
    }
    public string GetStatus() => $"Сохранено: {_storage.Count}; оповещений: {_alerts.Count}";
}
