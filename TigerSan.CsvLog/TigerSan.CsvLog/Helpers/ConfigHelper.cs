using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace TigerSan.CsvLog.Helpers
{
    public class ConfigHelper
    {
        #region 【Fields】
        /// <summary>配置路径</summary>
        private static string _path = string.Empty;
        /// <summary>应用启动路径</summary>
        public static readonly string _appStartupPath = GetStartupPath();
        #endregion 【Fields】

        #region 【Ctor】
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">配置路径</param>
        public ConfigHelper(string path)
        {
            _path = path;
            if (!File.Exists(_path))
            {
                throw new FileNotFoundException($"配置文件不存在: {_path}");
            }
        }
        #endregion 【Ctor】

        #region 【Functions】
        #region [Private]
        #region 获取“启动路径”
        /// <summary>获取“启动路径”</summary>
        private static string GetStartupPath()
        {
            try
            {
                return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return AppContext.BaseDirectory;
            }
        }
        #endregion
        #endregion [Private]

        #region 获取值
        public string? GetValue(string xpath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(_path);
                var node = doc.SelectSingleNode(xpath);

                if (node == null)
                {
                    LogHelper.HelperError($"{nameof(node)} is null.");
                    return null;
                }

                if (node.Attributes == null)
                {
                    LogHelper.HelperError($"{nameof(node.Attributes)} is null.");
                    return null;
                }

                var attrValue = node.Attributes["value"];

                if (attrValue == null)
                {
                    LogHelper.HelperError($"{nameof(attrValue)} is null.");
                    return null;
                }

                return attrValue.Value;
            }
            catch (Exception e)
            {
                LogHelper.HelperError(e.Message);
                return null;
            }
        }
        #endregion
        #endregion 【Functions】
    }
}
