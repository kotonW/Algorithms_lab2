using AlgorithmsLab2.Application.Settings;
using Xunit;

namespace AlgorithmsLab2.Tests;

public sealed class JsonSettingsStoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        "AlgorithmsLab2.Tests",
        Guid.NewGuid().ToString("N"));

    private string SettingsPath => Path.Combine(_directory, "settings.json");

    [Fact]
    public void Load_WithoutFile_ReturnsDefaults()
    {
        var store = new JsonSettingsStore(SettingsPath);

        var settings = store.Load();

        Assert.Equal(new AppSettings(), settings);
    }

    [Fact]
    public void SaveThenLoad_RoundTripsAllValues()
    {
        var store = new JsonSettingsStore(SettingsPath);
        var expected = new AppSettings
        {
            Task = "hanoi",
            Depth = 7,
            Palette = "Океан",
            DarkTheme = false,
            Progressive = true,
            DiskCount = 6,
            StepDelayMs = 320,
        };

        store.Save(expected);
        var actual = store.Load();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Save_CreatesDirectory()
    {
        var store = new JsonSettingsStore(SettingsPath);

        store.Save(new AppSettings());

        Assert.True(File.Exists(SettingsPath));
    }

    [Fact]
    public void Load_CorruptedFile_ReturnsDefaults()
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(SettingsPath, "{ это не json ");
        var store = new JsonSettingsStore(SettingsPath);

        var settings = store.Load();

        Assert.Equal(new AppSettings(), settings);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
