using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Preference;

namespace WozAlboPrzewoz
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);
        }

        public override bool OnPreferenceTreeClick(Preference preference)
        {
            if (preference.Key == "setting_licenses")
            {
                StartActivity(new Intent(Activity, typeof(LicensesActivity)));
            } else if(preference.Key == "dark_mode")
            {
                var switchPref = preference as SwitchPreference;
                AppCompatDelegate.DefaultNightMode = (switchPref.Checked ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo);
                //Activity.Recreate();
            }

            return base.OnPreferenceTreeClick(preference);
        }
    }
}