using Fujitsu.SLM.Web.Properties;

namespace Fujitsu.SLM.Web.Helpers
{
    public static class SettingsHelper
    {
        public static string VersionTitle => Settings.Default.VersionTitle;

        public static int UserLogonValidateInterval => Settings.Default.UserLogonValidateInterval;

        public static int CookieTimeout => Settings.Default.CookieTimeout;

        public static string CookieName => Settings.Default.CookieName;

        public static string Environment => Settings.Default.Environment;

        public static string ImportSpreadsheetFileLocation => Settings.Default.ImportSpreadsheetFileLocation;

        public static string HelpVideoFileLocation => Settings.Default.HelpVideoFileLocation;
    }
}
