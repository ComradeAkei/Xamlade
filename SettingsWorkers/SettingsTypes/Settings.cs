using System.Collections;
using System.Collections.Generic;

namespace Xamlade.SettingsWorkers.SettingsTypes;

public class Settings : IEnumerable<KeyValuePair<string, string>>
{
    private Dictionary<string, string> _settings = new Dictionary<string, string>();

    public string this[string key]
    {
        get => _settings.TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
        set => _settings[key] = value;
    }

    public void SetDictionary(Dictionary<string, string> dictionary) => _settings = dictionary;
    public void Add(string key, string value) => _settings[key] = value;
    public bool Remove(string key) => _settings.Remove(key);
    public bool TryGetValue(string key, out string value) => _settings.TryGetValue(key, out value);
    public void Clear() => _settings.Clear();

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _settings.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}