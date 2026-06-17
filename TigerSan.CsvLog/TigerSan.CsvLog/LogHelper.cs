using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using TigerSan.CsvOperation;
using TigerSan.CsvLog.Helpers;
using TigerSan.CsvLog.Settings;

namespace TigerSan.CsvLog
{
    public class LogHelper : IDisposable
    {
        #region 【Fields】
        #region [Private]
        private static readonly string LOG = "LOG";
        private static readonly string ERROR = "ERROR";
        private static readonly string WARNING = "WARNING";
        private static readonly string LogHelperError = "LogHelperError_";
        /// <summary>默认颜色</summary>
        private static readonly ConsoleColor _defaultColor = Console.ForegroundColor;
        /// <summary>是否已销毁</summary>
        private bool _disposed = false;
        /// <summary>CSV助手</summary>
        private readonly CsvHelper<LogData> _csvHelper;
        /// <summary>写入任务</summary>
        private Task? _writeTask;
        /// <summary>取消令牌</summary>
        private CancellationTokenSource? _cts;
        /// <summary>日志队列</summary>
        private ConcurrentQueue<LogData> _logQueue = new ConcurrentQueue<LogData>();
        #endregion [Private]

        /// <summary>日志目录</summary>
        public string _logDir = string.Empty;
        /// <summary>文件名</summary>
        public string _fileName = string.Empty;
        #endregion 【Fields】

        #region 【Properties】
        /// <summary>实例</summary>
        public static LogHelper Instance { get; } = new LogHelper();
        /// <summary>文件路径</summary>
        private string FilePath { get => $"{_logDir}\\{_fileName}{DateTime.Now.ToString("yyyy-MM-dd")}.csv"; }
        #endregion 【Properties】

        #region 【Ctor】
        public LogHelper()
        {
            Init();
            _csvHelper = new CsvHelper<LogData>(FilePath);
            StartWriteTask();
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
                    HelperError($"The path is unavailable!{Environment.NewLine}\"{CsvLogSetting.LogDir}\"");
                    _logDir = $"{ConfigHelper._appStartupPath}\\Log";
                }
            }
            else
            {
                _logDir = $"{ConfigHelper._appStartupPath}\\Log";
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
            #endregion FileName
        }
        #endregion

        #region 写入CSV
        /// <summary>写入CSV</summary>
        private void WriteCsv(LogData record)
        {
            if (_disposed) return;
            if (!Directory.Exists(_logDir))
                Directory.CreateDirectory(_logDir);

            try
            {
                _logQueue.Enqueue(record); // 放入队列
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
            var helper = new LogHelper()
            {
                _fileName = LogHelperError
            };

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
        public string Log(string? log,
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
            return data.Log;
        }
        #endregion

        #region 警告日志
        /// <summary>警告日志</summary>
        public string Warning(string? log,
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
            return data.Log;
        }
        #endregion

        #region 错误日志
        /// <summary>错误日志</summary>
        public string Error(string? log,
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
            return data.Log;
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

        #region 启动写入任务
        public void StartWriteTask()
        {
            Dispose();

            // 重新创建取消令牌
            _cts = new CancellationTokenSource();

            _writeTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (_logQueue.TryDequeue(out var data))
                    {
                        try
                        {
                            _csvHelper._path = FilePath;
                            var res = await _csvHelper.AppendAsync(data);
                            if (!res.IsSuccess)
                            {
                                Console.Error.WriteLine(res.Message);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine($"LogHelper: {e.Message}");
                        }
                    }
                    else
                    {
                        await Task.Delay(50); // 短暂等待，避免空转
                    }
                }
            });

            // 未销毁：
            _disposed = false;
        }
        #endregion

        #region 销毁
        public void Dispose()
        {
            if (_disposed) return;

            // 取消异步写入任务
            _cts?.Cancel();

            // 等待任务完成（设置10秒超时避免死锁）
            if (_writeTask != null)
            {
                try
                {
                    _writeTask.Wait(TimeSpan.FromSeconds(10));
                }
                catch (AggregateException)
                {
                    // 忽略任务异常
                }
            }

            // 释放托管资源
            _cts?.Dispose();

            // 重置字段
            _writeTask = null;
            _cts = null;

            // 已销毁：
            _disposed = true;
        }
        #endregion

        #region [附加]
        #region 为Null
        /// <summary>为Null</summary>
        public string IsNull(string name,
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
            return data.Log;
        }
        #endregion

        #region 为空
        /// <summary>为空</summary>
        public string IsEmpty(string name,
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
            return data.Log;
        }
        #endregion

        #region 为Null或空
        /// <summary>为Null或空</summary>
        public string IsNullOrEmpty(string name,
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
            return data.Log;
        }
        #endregion

        #region 超出范围
        /// <summary>超出范围</summary>
        public string IsOutOfRange(string name,
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
            return data.Log;
        }
        #endregion

        #region 不包含
        /// <summary>不包含</summary>
        public string IsNotContain(
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
            return data.Log;
        }
        #endregion

        #region 打印彩色文本
        /// <summary>打印彩色文本</summary>
        public void ColorWriteLine(string log, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(log);
            Console.ForegroundColor = _defaultColor;
        }
        #endregion
        #endregion [附加]
        #endregion 【Functions】
    }
}
