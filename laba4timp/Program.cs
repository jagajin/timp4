using System;
using System.Xml.Linq;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string mode = args.Length == 0 ? "all" : args[0].ToLowerInvariant();
        if (mode == "all" || mode == "decorator") DemoDecorator();
        if (mode == "all" || mode == "adapter") DemoAdapter();
        if (mode == "all" || mode == "facade") DemoFacade();
        if (mode != "all" && mode != "decorator" && mode != "adapter" && mode != "facade")
            throw new ArgumentException("Режимы: all, decorator, adapter, facade");
    }
    private static void Check(string name, bool condition)
    {
        Console.WriteLine($"{name}: {(condition ? "PASS" : "FAIL")}");
        if (!condition) throw new InvalidOperationException(name);
    }
    private static void DemoDecorator()
    {
        Console.WriteLine("Decorator");
        var time = new DateTime(2026, 10, 29, 10, 0, 0);
        LogEntry original = new SystemLogEntry(time, "Служба запущена", "Auth");
        LogEntry severity = new SeverityDecorator(original, "HIGH");
        LogEntry source = new SourceDecorator(severity, "server-01");
        LogEntry decorated = new XmlFormatDecorator(source);
        string record = decorated.GetRecord(); //dek local
        Console.WriteLine(record);
        string expected = "<log>[источник: server-01] [важность: HIGH] [2026-10-29 10:00:00] Служба запущена (компонент: Auth)</log>";
        Check("D1 Цепочка трёх декораторов", record == expected);
        Check("D2 Исходная запись не изменена", original.GetRecord() == "[2026-10-29 10:00:00] Служба запущена (компонент: Auth)");
        LogEntry security = new SecurityLogEntry(time, "Отказ <root> & guest", "LoginFailed");
        LogEntry escaped = new XmlFormatDecorator(security);
        string xml = escaped.GetRecord();
        Check("D3 XML сохраняет спецсимволы", XElement.Parse(xml).Value == security.GetRecord());
        Check("D4 Сообщение и дата сохранены", decorated.GetMessage() == original.GetMessage() && decorated.CreatedAt == time);
        string reverse = new SeverityDecorator(new SourceDecorator(original, "server-01"), "HIGH").GetRecord();
        Check("D5 Порядок обёрток влияет на строку", reverse != source.GetRecord());
    }
    private static void DemoAdapter()
    {
        Console.WriteLine("Adapter");
        var client = new SyslogClient();
        ILogSink sink = new SyslogAdapter(client, 3);
        var service = new LogService(sink);
        LogEntry entry = new SecurityLogEntry(new DateTime(2026, 10, 29, 10, 0, 0), "Неудачный вход", "LoginFailed");
        service.Log(entry); //adapter local
    }
    private static void DemoFacade()
    {
        Console.WriteLine("Facade");
        var facade = new LoggingFacade();
        var time = new DateTime(2026, 10, 29, 10, 0, 0);
        LogEntry regular = new SystemLogEntry(time, "Служба запущена", "Auth");
        LogEntry incident = new SecurityLogEntry(time, "Неудачный вход", "LoginFailed");
        LogEntry empty = new SystemLogEntry(time, "   ", "Auth");
        Check("F1 Обычная запись принята", facade.Log(regular, false));
        Check("F2 Запись с оповещением принята", facade.Log(incident, true));
        Check("F3 Пустая запись отклонена", !facade.Log(empty, true));
        string status = facade.GetStatus(); // facade local
        Console.WriteLine(status);
        Check("F4 Две записи и одно оповещение", status == "Сохранено: 2; оповещений: 1");
    }
}
