using wguterms.Classes;
using SQLite;

using System;
using System.Collections.Generic;
using System.Net.Mail;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace wguterms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCourse : ContentPage
    {
        public Term termPage;
        public MainPage mainPage;
        Dictionary<string, bool> notificationsDict = new Dictionary<string, bool>

        {
            { "Yes",true},
            {"No",false}
        };

        public AddCourse(Term term, MainPage main)
        {
            termPage = term;
            mainPage = main;
            InitializeComponent();
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (ValidateUserInput())
            {

                Course newCourse = new Course();
                newCourse.CourseName = txtCourseTitle.Text;
                newCourse.CourseStatus = pickerCourseStatus.SelectedItem.ToString();
                newCourse.Start = dpStartDate.Date;
                newCourse.End = dpEndDate.Date;
                newCourse.InstructorName = txtCourseInstructorName.Text;
                newCourse.InstructorEmail = txtInstructorEmail.Text;
                newCourse.InstructorPhone = txtInstructorPhone.Text;
                newCourse.Notes = txtNotes.Text;
                newCourse.GetNotified = pickerNotifications.SelectedIndex;
                newCourse.Term = termPage.Id;



                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.Insert(newCourse);

                    // Maybe don't have to update termListView if OnAppearing() gets called when this modal 
                    // is dismissed.....yes we do lol, even though documentation says that OnAppearing() gets
                    // called when modal is dismissed.  bug? 
                    //https://forums.xamarin.com/discussion/58606/onappearing-not-called-on-android-for-underneath-page-if-page-on-top-was-pushed-modal
                    mainPage.courses.Add(newCourse);
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                await Navigation.PushModalAsync(new InputError());
            }

        }

        private void btnExit_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private bool ValidateUserInput()
        {
            bool valid = true;

            if (txtCourseTitle == null ||
                pickerCourseStatus.SelectedItem == null ||
                dpStartDate.Date == null ||
                dpEndDate.Date == null ||
                dpEndDate.Date < dpStartDate.Date ||
                txtCourseInstructorName == null ||
                txtInstructorEmail.Text == null ||
                txtInstructorPhone == null ||
                pickerNotifications.SelectedItem == null
                )

            {
                return false;
            }

            if (txtInstructorEmail.Text != null)
            {
                valid = ValidateEmail(txtInstructorEmail.Text);
            }


            return valid;
        }
        private bool ValidateEmail(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
