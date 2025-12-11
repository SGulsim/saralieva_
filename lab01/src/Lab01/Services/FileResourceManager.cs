using System.Text;

namespace Lab1.Services;

public class FileResourceManager : IDisposable
{
    private FileStream? _fileStream;
    private StreamWriter? _writer;
    private StreamReader? _reader;
    private bool _disposed;
    private readonly string _filePath;

    public FileResourceManager(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("путь к файлу не может быть пустым", nameof(filePath));
        }

        _filePath = filePath;
    }

    public void OpenForWriting(bool append = false)
    {
        ThrowIfDisposed();
        DisposeStreams();

        EnsureDirectoryExists();

        _fileStream = new FileStream(
            _filePath,
            append ? FileMode.Append : FileMode.Create,
            FileAccess.Write,
            append ? FileShare.Read : FileShare.None);

        _writer = new StreamWriter(_fileStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true);
    }

    public void OpenForReading()
    {
        ThrowIfDisposed();
        DisposeStreams();

        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException("файл не найден", _filePath);
        }

        _fileStream = new FileStream(
            _filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        _reader = new StreamReader(_fileStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
    }

    public void WriteLine(string text)
    {
        ThrowIfDisposed();

        if (_writer is null)
        {
            throw new InvalidOperationException("файл не открыт для записи. Сначала вызовите OpenForWriting()");
        }

        _writer.WriteLine(text);
        _writer.Flush();
    }

    public string ReadAllText()
    {
        ThrowIfDisposed();

        if (_reader is null)
        {
            throw new InvalidOperationException("файл не открыт для чтения. Сначала вызовите OpenForReading()");
        }
        // сначала сбрасываем буфер, чтобы читать с начала
        _reader.DiscardBufferedData();
        _reader.BaseStream.Seek(0, SeekOrigin.Begin);

        return _reader.ReadToEnd();
    }

    public void AppendText(string text)
    {
        ThrowIfDisposed();

        EnsureDirectoryExists();

        using var stream = new FileStream(
            _filePath,
            FileMode.Append,
            FileAccess.Write,
            FileShare.Read);

        using var writer = new StreamWriter(stream, new UTF8Encoding(false));
        writer.Write(text);
        writer.Flush();
    }

    public FileInfo GetFileInfo()
    {
        ThrowIfDisposed();
        return new FileInfo(_filePath);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _writer?.Dispose();
            _reader?.Dispose();
            _fileStream?.Dispose();
        }

        _writer = null;
        _reader = null;
        _fileStream = null;
        _disposed = true;
    }

    ~FileResourceManager()
    {
        Dispose(disposing: false);
    }

    private void DisposeStreams()
    {
        _writer?.Dispose();
        _reader?.Dispose();
        _fileStream?.Dispose();

        _writer = null;
        _reader = null;
        _fileStream = null;
    }

    private void EnsureDirectoryExists()
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(_filePath));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FileResourceManager));
        }
    }
}
