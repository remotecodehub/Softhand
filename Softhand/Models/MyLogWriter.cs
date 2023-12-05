using pjsua2xamarin.pjsua2;

namespace Softhand.Models;

public class MyLogWriter : LogWriter
{
    override public void write(LogEntry entry)
    {
        Console.WriteLine(entry.msg);
    }
}