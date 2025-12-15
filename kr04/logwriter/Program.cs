namespace logwriter;
class LogWriter : IDisposable
{
    private readonly FileStream _fileStream;
    private bool _disposed = false;

    public LogWriter(string filePath)
    {
        _fileStream = new FileStream(filePath, FileMode.Append, 
            FileAccess.Write, FileShare.Read);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _fileStream?.Dispose();
        }
        
        _disposed = true;
    }

    ~LogWriter()
    {
        Dispose(false);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var file = new LogWriter("D:/doklad_site_visual.docx");
        Console.WriteLine($"файлик до вызова диспоуз: {file.ToString()}");
        Console.WriteLine($"файлик после вызова диспоуз (он ваще автоматически вызывается):  {file.ToString()}");
    }
}