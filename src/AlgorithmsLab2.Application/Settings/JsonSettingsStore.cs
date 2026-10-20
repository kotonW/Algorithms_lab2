using System.Text.Json;

namespace AlgorithmsLab2.Application.Settings;

/// <summary>
/// Хранит настройки в JSON-файле. Любая ошибка чтения или записи не должна ломать запуск:
/// удобство важнее, чем строгость.
/// </summary>
public sealed class JsonSettingsStore : ISettingsStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    private readonly string _filePath;

    /// <param name="filePath">Путь к файлу настроек; по умолчанию — в профиле пользователя.</param>
    public JsonSettingsStore(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AlgorithmsLab2",
            "settings.json");
    }

    /// <summary>Путь к файлу настроек.</summary>
    public string FilePath => _filePath;

    /// <inheritdoc />
    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<AppSettings>(json, SerializerOptions) ?? new AppSettings();
        }
        catch (Exception exception) when (exception is IOException
                                              or UnauthorizedAccessException
                                              or JsonException
                                              or ArgumentException)
        {
            return new AppSettings();
        }
    }

    /// <inheritdoc />
    public void Save(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        try
        {
            var directory = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(_filePath, JsonSerializer.Serialize(settings, SerializerOptions));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // Настройки — удобство, а не критичная функция.
        }
    }
}
