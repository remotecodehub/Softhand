using pjsua2maui.pjsua2;

namespace Softhand.Models;

public class SoftLogWriter : LogWriter
{
    override public void write(LogEntry entry)
    {
        Console.WriteLine(entry.msg);
    }
}