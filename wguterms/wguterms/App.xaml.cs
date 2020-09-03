
using Xamarin.Forms;

namespace wguterms
{
    public partial class App : Application
    {
       
        public static string FilePath;
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromHex("#444"),
                BarTextColor = Color.White,
            };
        }

        public App(string filePath)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromHex("#444"),
                BarTextColor = Color.White,
            };
            FilePath = filePath;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
    
}
