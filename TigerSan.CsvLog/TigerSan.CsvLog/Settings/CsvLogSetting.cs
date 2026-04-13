using System.IO;
using TigerSan.CsvLog.Helpers;

namespace TigerSan.CsvLog.Settings
{
    public static class CsvLogSetting
    {
        #region 【Fields】
        private static readonly string? _appStartupPath = Path.GetDirectoryName(Environment.ProcessPath);
        private static readonly string _configPath = $"{_appStartupPath}\\CsvLog.config";
        private static readonly ConfigHelper? _config;
        private static readonly string _baseXpath = "/configuration/logSettings";
        #endregion 【Fields】

        #region 【Properties】
        /// <summary>
        /// 保存Log的文件夹
        /// </summary>
        public static string? LogDir
        {
            get => GetValue(nameof(LogDir));
        }

        /// <summary>
        /// Log的文件名前缀
        /// </summary>
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
                File.WriteAllBytes(_configPath, Resources.CsvLogConfig);
            }
            _config = new ConfigHelper(_configPath);
        }
        #endregion 【Ctor】
    }
}
