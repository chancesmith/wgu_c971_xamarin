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
            if (IsUserInputValid())
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

                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.Insert(newCourse);
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

        private bool IsUserInputValid()
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
                valid = IsEmailValid(txtInstructorEmail.Text);
            }


            return valid;
        }
        private bool IsEmailValid(string email)
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
