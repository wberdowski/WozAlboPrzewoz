namespace WozAlboPrzewoz
{
    public class License
    {
        public string Product { get; set; }
        public string LicenseText { get; set; }

        public License()
        {

        }

        public License(string product, string licenseText)
        {
            Product = product;
            LicenseText = licenseText;
        }
    }
}