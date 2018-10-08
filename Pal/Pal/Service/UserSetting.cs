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

        public static bool IsUserSet => AppSettings.Contains(nameof(UserToken));

        public static string UserToken
        {
            get => AppSettings.GetValueOrDefault(nameof(UserToken),string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserToken), value);
        }

        public static void RemoveUserToken() => AppSettings.Remove(nameof(UserToken));

        public static void ClearEverything()
        {
            AppSettings.Clear();
        }
    }

}
