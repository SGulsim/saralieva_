using Lab1.Services;
using Xunit;

namespace Lab01.Tests;

public class FileResourceManagerTests : IDisposable
{
    private readonly string _tempDirectory;

    public FileResourceManagerTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"Lab01FileManager_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
    }

    [Fact]
    public void OpenForWriting_ЗаписываетФайл_КоторыйЧитаетсяЧерезOpenForReading()
    {
        var path = GetTempFilePath("notes.txt");

        using (var manager = new FileResourceManager(path))
        {
            manager.OpenForWriting();
            manager.WriteLine("первая строка");
            manager.WriteLine("вторая строка");
        }

        using var readerManager = new FileResourceManager(path);
        readerManager.OpenForReading();
        var content = readerManager.ReadAllText();

        Assert.Contains("первая строка", content);
        Assert.Contains("вторая строка", content);
    }

    [Fact]
    public void AppendText_ДобавляетБезПерезаписи()
    {
        var path = GetTempFilePath("append.txt");

        using (var manager = new FileResourceManager(path))
        {
            manager.OpenForWriting();
            manager.WriteLine("base");
        }

        using (var manager = new FileResourceManager(path))
        {
            manager.AppendText(" + tail");
        }

        using var readerManager = new FileResourceManager(path);
        readerManager.OpenForReading();
        var content = readerManager.ReadAllText();

        Assert.Contains("base", content);
        Assert.Contains("tail", content);
    }

    [Fact]
    public void OpenForReading_ЕслиФайлОтсутствует_ГенерируетFileNotFoundException()
    {
        var path = GetTempFilePath("missing.txt");

        using var manager = new FileResourceManager(path);

        Assert.Throws<FileNotFoundException>(() => manager.OpenForReading());
    }

    [Fact]
    public void ПослеDispose_ЛюбыеОперацииГенерируютObjectDisposedException()
    {
        var path = GetTempFilePath("disposed.txt");
        var manager = new FileResourceManager(path);
        manager.Dispose();

        Assert.Throws<ObjectDisposedException>(() => manager.OpenForWriting());
        Assert.Throws<ObjectDisposedException>(() => manager.OpenForReading());
    }

    private string GetTempFilePath(string fileName) => Path.Combine(_tempDirectory, fileName);

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }
}
