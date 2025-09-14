using pjsua2maui.pjsua2;

namespace Softhand.Domain.Models;

public class SoftLogWriter : LogWriter
{
    override public void write(LogEntry entry)
    {
        Console.WriteLine(entry.msg);
    }
}