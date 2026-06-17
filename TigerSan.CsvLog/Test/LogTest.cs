using TigerSan.CsvLog;

namespace Test
{
    public static class LogTest
    {
        public static void WriteLog()
        {
            LogHelper.HelperError("Helper Error!");
            LogHelper.Instance.Log("Log test.");
            LogHelper.Instance.Warning("Warning test.");
            LogHelper.Instance.Error("Log test.");
            LogHelper.Instance.IsNull("value");
            LogHelper.Instance.IsEmpty("value");
            LogHelper.Instance.IsNullOrEmpty("value");
            LogHelper.Instance.IsOutOfRange("value");
            LogHelper.Instance.IsNotContain("list", "item");
        }
    }
}
