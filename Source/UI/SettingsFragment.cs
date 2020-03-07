using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Preference;
using System.Reflection;

namespace WozAlboPrzewoz
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);

            var versionPref = FindPreference("setting_version");
            versionPref.Summary = Assembly.GetExecutingAssembly().GetName().Version.ToString();

#if DEBUG
            versionPref.Summary += "\n" + GetString(Resource.String.setting_debug_mode);
#endif
        }

        public override bool OnPreferenceTreeClick(Preference preference)
        {
            if (preference.Key == "setting_licenses")
            {
                StartActivity(new Intent(Activity, typeof(LicensesActivity)));
            }
            else if (preference.Key == "dark_mode")
            {
                var switchPref = preference as SwitchPreference;
                AppCompatDelegate.DefaultNightMode = (switchPref.Checked ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo);
                //Activity.Recreate();
            }

            return base.OnPreferenceTreeClick(preference);
        }
    }
}