using CsvHelper;
using System.Globalization;
using System.Runtime.CompilerServices;
using TigerSan.CsvLog.Settings;

namespace TigerSan.CsvLog
{
    public class LogHelper
    {
        #region 【Fields】
        #region [Private]
        private static readonly string LOG = "LOG";
        private static readonly string ERROR = "ERROR";
        private static readonly string WARNING = "WARNING";
        private static readonly string LogHelperError = "LogHelperError_";
        #endregion [Private]

        /// <summary>日志目录</summary>
        public string _logDir = string.Empty;
        /// <summary>文件名</summary>
        public string _fileName = string.Empty;
        /// <summary>应用启动路径</summary>
        public static readonly string? _appStartupPath = Path.GetDirectoryName(Environment.ProcessPath);
        #endregion 【Fields】

        #region 【Properties】
        /// <summary>文件路径</summary>
        private string FilePath { get => $"{_logDir}\\{_fileName}{DateTime.Now.ToString("yyyy-MM-dd")}.csv"; }
        /// <summary>实例</summary>
        public static LogHelper Instance { get; } = new LogHelper();
        #endregion 【Properties】

        #region 【Ctor】
        public LogHelper()
        {
            Init();
        }
        #endregion 【Ctor】

        #region 【Functions】
        #region [Private]
        #region 初始化
        /// <summary>初始化</summary>
        private void Init()
        {
            #region LogDir
            if (CsvLogSetting.LogDir != null
                && !string.IsNullOrEmpty(CsvLogSetting.LogDir))
            {
                _logDir = CsvLogSetting.LogDir;

                if (!Directory.Exists(_logDir))
                {
                    Directory.CreateDirectory(_logDir);
                }
                if (!Directory.Exists(_logDir))
                {
                    HelperError($"路径不可用：\r\n“{CsvLogSetting.LogDir}”");
                    _logDir = $"{_appStartupPath}\\Log";
                }
            }
            else
            {
                _logDir = $"{_appStartupPath}\\Log";
            }
            #endregion LogDir

            #region FileName
            if (CsvLogSetting.FileName != null
                && !string.IsNullOrEmpty(CsvLogSetting.FileName))
            {
                _fileName = CsvLogSetting.FileName;
            }
            else
            {
                _fileName = "log_";
            }
            #endregion LogDir
        }
        #endregion

        #region 写入CSV
        /// <summary>写入CSV</summary>
        private void WriteCsv(LogData record)
        {
            if (!Directory.Exists(_logDir))
                Directory.CreateDirectory(_logDir);

            List<LogData> records;

            try
            {
                #region 初始化“记录集合”
                if (!File.Exists(FilePath))
                {
                    records = new List<LogData>();
                }
                else
                {
                    using (var reader = new StreamReader(FilePath))
                    using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        records = csvReader.GetRecords<LogData>().ToList();
                    }
                }
                #endregion 初始化“记录集合”

                #region 写入“新记录”
                using (var writer = new StreamWriter(FilePath))
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    records.Add(record);
                    csvWriter.WriteRecords(records);
                }
                #endregion 写入“新记录”
            }
            catch (Exception e)
            {
                HelperError(e.Message);
            }
        }
        #endregion

        #region 获取“日志字符串”
        /// <summary>获取“日志字符串”</summary>
        private static string GetLogString(LogData data)
        {
            return $"{data.Time}\t\n" +
                $"{nameof(data.Type)}: \"{data.Type}\"\r\n" +
                $"{nameof(data.FilePath)}: \"{data.FilePath}\"\r\n" +
                $"{nameof(data.MemberName)}: \"{data.MemberName}\"\r\n" +
                $"{nameof(data.LineNumber)}: {data.LineNumber}\r\n" +
                $"{nameof(data.Log)}: \"{data.Log}\"\r\n";
        }
        #endregion
        #endregion [Private]

        #region LogHelper内部错误
        /// <summary>LogHelper内部错误</summary>
        public static void HelperError(string? log,
                   [CallerMemberName] string memberName = "",
                   [CallerFilePath] string filePath = "",
                   [CallerLineNumber] int lineNumber = -1)
        {
            var helper = new LogHelper();
            helper._fileName = LogHelperError;

            var data = new LogData()
            {
                Type = ERROR,
                MemberName = memberName,
                FilePath = filePath,
                LineNumber = lineNumber,
                Log = log ?? string.Empty,
            };

            helper.WriteCsv(data);
            GetLogString(data);
        }
        #endregion

        #region 普通日志
        /// <summary>普通日志</summary>
        public void Log(string? log,
                   [CallerMemberName] string memberName = "",
                   [CallerFilePath] string filePath = "",
                   [CallerLineNumber] int lineNumber = -1)
        {
            var data = new LogData()
            {
                Type = LOG,
                MemberName = memberName,
                FilePath = filePath,
                LineNumber = lineNumber,
                Log = log ?? string.Empty,
            };

            WriteCsv(data);
            Console.WriteLine(GetLogString(data));
        }
        #endregion

        #region 警告日志
        /// <summary>警告日志</summary>
        public void Warning(string? log,
                   [CallerMemberName] string memberName = "",
                   [CallerFilePath] string filePath = "",
                   [CallerLineNumber] int lineNumber = -1)
        {
            var data = new LogData()
            {
                Type = WARNING,
                MemberName = memberName,
                FilePath = filePath,
                LineNumber = lineNumber,
                Log = log ?? string.Empty,
            };

            WriteCsv(data);
            ColorWriteLine(GetLogString(data), ConsoleColor.Yellow);
        }
        #endregion

        #region 错误日志
        /// <summary>错误日志</summary>
        public void Error(string? log,
                   [CallerMemberName] string memberName = "",
                   [CallerFilePath] string filePath = "",
                   [CallerLineNumber] int lineNumber = -1)
        {
            var data = new LogData()
            {
                Type = ERROR,
                MemberName = memberName,
                FilePath = filePath,
                LineNumber = lineNumber,
                Log = log ?? string.Empty,
            };

            WriteCsv(data);
            ColorWriteLine(GetLogString(data), ConsoleColor.Red);
        }
        #endregion

        #region 删除当天的日志
        /// <summary>删除当天的日志</summary>
        public void DeleteLog()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
        #endregion

        #region 删除日志文件夹
        /// <summary>删除日志文件夹</summary>
        public void DeleteFolder()
        {
            if (Directory.Exists(_logDir))
                Directory.Delete(_logDir, true);
        }
        #endregion

        #region [附加]
        #region 为Null
        /// <summary>为Null</summary>
        public void IsNull(string name,
                   [CallerMemberName] string memberName = "",
                   [CallerFilePath] string filePath = "",
                   [CallerLineNumber] int lineNumber = -1)
        {
            var data = new LogData()
            {
                Type = WARNING,
                MemberName = memberName,
                FilePath = filePath,
                LineNumber = lineNumber,
                Log = $"The {name} is null!",
            };

            WriteCsv(data);
            ColorWriteLine(GetLogString(data), ConsoleColor.Yellow);
        }
        #endregion

        #region 为空
        /// <summary>为空</summary>
        public void IsEmpty(string name,
                   [CallerMemberName] string memberName = "",
                   [CallerFilePath] string filePath = "",
                   [CallerLineNumber] int lineNumber = -1)
        {
            var data = new LogData()
            {
                Type = WARNING,
                MemberName = memberName,
                FilePath = filePath,
                LineNumber = lineNumber,
                Log = $"The {name} is empty!",
            };

            WriteCsv(data);
            ColorWriteLine(GetLogString(data), ConsoleColor.Yellow);
        }
        #endregion

        #region 为Null或空
        /// <summary>为Null或空</summary>
        public void IsNullOrEmpty(string name,
                   [CallerMemberName] string memberName = "",
                   [CallerFilePath] string filePath = "",
                   [CallerLineNumber] int lineNumber = -1)
        {
            var data = new LogData()
            {
                Type = WARNING,
                MemberName = memberName,
                FilePath = filePath,
                LineNumber = lineNumber,
                Log = $"The {name} is null or empty!",
            };

            WriteCsv(data);
            ColorWriteLine(GetLogString(data), ConsoleColor.Yellow);
        }
        #endregion

        #region 超出范围
        /// <summary>超出范围</summary>
        public void IsOutOfRange(string name,
                   [CallerMemberName] string memberName = "",
                   [CallerFilePath] string filePath = "",
                   [CallerLineNumber] int lineNumber = -1)
        {
            var data = new LogData()
            {
                Type = WARNING,
                MemberName = memberName,
                FilePath = filePath,
                LineNumber = lineNumber,
                Log = $"The {name} is out of range!",
            };

            WriteCsv(data);
            ColorWriteLine(GetLogString(data), ConsoleColor.Yellow);
        }
        #endregion

        #region 不包含
        /// <summary>不包含</summary>
        public void IsNotContain(
            string listName,
            string itemName,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1)
        {
            var data = new LogData()
            {
                Type = WARNING,
                MemberName = memberName,
                FilePath = filePath,
                LineNumber = lineNumber,
                Log = $"The {listName} does not contain {itemName}!",
            };

            WriteCsv(data);
            ColorWriteLine(GetLogString(data), ConsoleColor.Yellow);
        }
        #endregion

        #region 打印彩色文本
        /// <summary>打印彩色文本</summary>
        public void ColorWriteLine(string log, ConsoleColor color)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(log);
            Console.ForegroundColor = previousColor;
        }
        #endregion
        #endregion [附加]
        #endregion 【Functions】
    }
}
