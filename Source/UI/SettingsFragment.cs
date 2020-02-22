using Android.Content;
using Android.OS;
using Android.Widget;
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
            }

            return base.OnPreferenceTreeClick(preference);
        }
    }
}