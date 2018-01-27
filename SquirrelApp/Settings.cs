using System;
using System.IO;
using Newtonsoft.Json;

namespace SquirrelApp
{
    /// <summary>
    /// Class Settings.
    /// </summary>
    public class Settings
    {
        private string _version;
        private int _years;
        private static Settings _settings;
        private Settings()
        {
        }

        public static Settings Current
        {
            get
            {
                if (_settings == null)
                {
                    var path = GetPath();
                    if (!File.Exists(path))
                    {
                        _settings = new Settings();
                    }
                    else
                    {
                        var content = File.ReadAllText(path);
                        _settings = JsonConvert.DeserializeObject<Settings>(content);
                    }
                }

                return _settings;
            }
        }

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public int Years
        {
            get { return _years; }
            set { _years = value; }
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(GetPath(), json);
        }

        private static string GetPath()
        {
            return Environment.CurrentDirectory + "/../setting.json";
        }
    }
}
