using wguterms.Classes;
using SQLite;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace wguterms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTerm : ContentPage
    {
        public MainPage mainPage;
        public AddTerm(MainPage main)
        {
            mainPage = main;
            InitializeComponent();
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (IsUserInputValid())
            {
                Term newTerm = new Term();

                newTerm.TermName = txtTermTitle.Text;
                newTerm.Start = dpStartDate.Date;
                newTerm.End = dpEndDate.Date;

                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.Insert(newTerm);
                    mainPage.terms.Add(newTerm);
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                await DisplayAlert("Alert", "All fields must be filled out. Email addresses must be valid. Start Date must be earlier than End Date.  Please try again.", "OK");
            }

        }
        private bool IsUserInputValid()
        {
            bool valid = true;

            if (txtTermTitle.Text == null ||
                dpStartDate.Date == null ||
                dpEndDate.Date == null ||
                dpEndDate.Date < dpStartDate.Date
                )

            {
                return false;
            }
            return valid;
        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}