using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Xamarin.Forms;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class ConnectionDetailsActivity : AppCompatActivity
    {
        private int r;
        private int z;
        private double dk;
        private int spnnt;
        private int sknnt;
        private TextView tv1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_connection_details);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            r = Intent.GetIntExtra("r", 0);
            z = Intent.GetIntExtra("z", 0);
            dk = Intent.GetDoubleExtra("dk", 0);
            spnnt = Intent.GetIntExtra("spnnt", 0);
            sknnt = Intent.GetIntExtra("sknnt", 0);

            Title = r + " " + z + " " + dk + " " + spnnt + " " + sknnt;

            tv1 = (TextView)FindViewById(Resource.Id.textView1);

            new System.Threading.Thread(() =>
            {
                var details = PKPAPI.GetConnectionRoute(r, z, dk, spnnt, sknnt);
                Device.BeginInvokeOnMainThread(() =>
                {
                    tv1.Text = details;
                });
            }).Start();
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }
    }
}