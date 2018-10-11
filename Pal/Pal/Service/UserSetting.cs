using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Pal.Service
{
    public static class UserSetting
    {

        private static ISettings AppSettings
        {
            get {
                if(CrossSettings.IsSupported)
                    return CrossSettings.Current;
                return null;
            }
        }

        public static bool IsUserEmail => AppSettings.Contains(nameof(UserEmail));
        public static bool IsUserName => AppSettings.Contains(nameof(UserName));

        public static string UserEmail
        {
            get => AppSettings.GetValueOrDefault(nameof(UserEmail),string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserEmail), value);
        }

        public static string UserName {
            get => AppSettings.GetValueOrDefault(nameof(UserName), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserName), value);

        }

        public static void RemoveUser()
        {
            AppSettings.Remove(nameof(UserEmail));
            AppSettings.Remove(nameof(UserName));
        }

        public static void ClearEverything()
        {
            AppSettings.Clear();
        }
    }

}
