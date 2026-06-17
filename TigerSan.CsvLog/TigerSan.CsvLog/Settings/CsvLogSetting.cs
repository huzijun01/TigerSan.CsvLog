using System;
using System.IO;
using System.Reflection;
using TigerSan.CsvLog.Helpers;

namespace TigerSan.CsvLog.Settings
{
    public static class CsvLogSetting
    {
        #region 【Fields】
        private static readonly ConfigHelper? _config;
        private static readonly string _baseXpath = "/configuration/logSettings";
        private static readonly string _configPath = $"{ConfigHelper._appStartupPath}\\CsvLog.config";
        #endregion 【Fields】

        #region 【Properties】
        /// <summary>保存Log的文件夹</summary>
        public static string? LogDir
        {
            get => GetValue(nameof(LogDir));
        }

        /// <summary>Log的文件名前缀</summary>
        public static string? FileName
        {
            get => GetValue(nameof(FileName));
        }
        #endregion 【Properties】

        #region 【Functions】
        private static string? GetValue(string key)
        {
            if (_config == null) return null;

            return _config.GetValue($"{_baseXpath}/add[@key='{key}']");
        }
        #endregion 【Functions】

        #region 【Ctor】
        static CsvLogSetting()
        {
            if (!File.Exists(_configPath))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = $"{assembly.GetName().Name}.Files.CsvLog.config";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        var names = assembly.GetManifestResourceNames();
                        throw new FileNotFoundException($"Resource not found: {resourceName}");
                    }

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var config = reader.ReadToEnd();
                        if (string.IsNullOrEmpty(config))
                        {
                            Console.WriteLine();
                        }
                        File.WriteAllText(_configPath, config);
                    }
                }
            }

            _config = new ConfigHelper(_configPath);
        }
        #endregion 【Ctor】
    }
}
