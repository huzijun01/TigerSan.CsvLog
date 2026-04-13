using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TigerSan.CsvLog
{
    #region 日志数据
    /// <summary>
    /// 日志数据
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// 时间格式
        /// </summary>
        public static readonly string _timeFormat = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 时间戳
        /// </summary>
        public string Time { get; set; } = DateTime.Now.ToString(_timeFormat);
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 成员名
        /// </summary>
        public string MemberName { get; set; } = string.Empty;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
        /// <summary>
        /// 行数
        /// </summary>
        public int LineNumber { get; set; } = -1;
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Log { get; set; } = string.Empty;
    }
    #endregion
}
