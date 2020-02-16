using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace VisualStudio
{
    public class ToolSettings
    {
        readonly string toolName;
        Dictionary<string, string> settings;

        public ToolSettings(string toolName) => this.toolName = toolName;

        public string Get(string setting) => Get<string>(setting, null);

        public T Get<T>(string setting, T defaultValue = default)
        {
            if (Settings.TryGetValue(setting, out var serialized))
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(serialized);

            return defaultValue;
        }

        public void Set<T>(string setting, T value)
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".net", "tools", toolName);

            // TODO: multi-level writing not supported yet. We just 
            // write to the global user store.

            Directory.CreateDirectory(path);
            var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var iniFile = Path.Combine(path, toolName + ".ini");
            if (File.Exists(iniFile))
            {
                foreach (var pair in File
                    .ReadAllLines(iniFile)
                    .Select(line => line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(pair => pair.Length == 2))
                {
                    settings[pair[0].Trim()] = pair[1].Trim();
                }
            }

            if (value == null)
            {
                if (settings.Remove(setting))
                    File.WriteAllLines(iniFile, settings.Select(pair => $"{pair.Key}={pair.Value}"));

                return;
            }
            else
            {
                var serialized = value.ToString();
                if (!settings.ContainsKey(setting) || settings[setting] != serialized)
                {
                    settings[setting] = value.ToString();
                    File.WriteAllLines(iniFile, settings.Select(pair => $"{pair.Key}={pair.Value}"));
                }
            }
        }

        public IEnumerable<KeyValuePair<string, string>> GetAll()
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }

        Dictionary<string, string> Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    var values = new List<(string key, string value)>();
                    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                    while (dir != null)
                    {
                        AddValues(values, Path.Combine(dir.FullName, ".net"));
                        dir = dir.Parent;
                    }

                    AddValues(values, Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            ".net", "tools"));

                    values.Reverse();
                    foreach (var pair in values)
                    {
                        settings[pair.key] = pair.value;
                    }
                }

                return settings;
            }
        }

        void AddValues(List<(string key, string value)> values, string directory)
        {
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
                return;

            var iniFile = Path.Combine(directory, toolName, toolName + ".ini");
            if (File.Exists(iniFile))
            {
                values.AddRange(File
                    .ReadAllLines(iniFile)
                    .Select(line => line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(pair => pair.Length == 2)
                    .Select(pair => (pair[0].Trim(), pair[1].Trim())));
            }
        }
    }
}
