using wguterms.Classes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace wguterms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoursePage : ContentPage
    {
        public Course _course;
        public Term _term;
        public MainPage _main;
        public List<string> pickerStates = new List<string> { "In Progress", "Completed", "Dropped", "Plan To Take" };
        public List<string> pickerNotificationsStates = new List<string> { "Yes", "No" };
        public CoursePage(Term term, MainPage main, Course course)
        {
            _course = course;
            _term = term;
            _main = main;
            InitializeComponent();

        }
        protected override void OnAppearing()
        {
            pickerCourseStatus.ItemsSource = pickerStates;
            pickerCourseStatus.SelectedIndex = pickerStates.FindIndex(status => status == _course.CourseStatus);
            txtCourseTitle.Text = _course.CourseName;
            pickerCourseStatus.SelectedItem = _course.CourseStatus;
            dpStartDate.Date = _course.Start.Date;
            dpEndDate.Date = _course.End.Date;
            txtInstructorName.Text = _course.InstructorName;
            txtInstructorPhone.Text = _course.InstructorPhone;
            txtInstructorEmail.Text = _course.InstructorEmail;
            txtNotes.Text = _course.Notes;
            if (_course.GetNotified == 0)
            {
                pickerNotifications.SelectedIndex = 0;
            }
            else
            {
                pickerNotifications.SelectedIndex = 1;
            }
            base.OnAppearing();
        }

        private async void btnEditCourse_Clicked(object sender, EventArgs e)
        {
            if (IsUserInputValid())
            {
                _course.CourseName = txtCourseTitle.Text;
                _course.CourseStatus = pickerCourseStatus.SelectedItem.ToString();
                _course.Start = dpStartDate.Date;
                _course.End = dpEndDate.Date;
                _course.InstructorName = txtInstructorName.Text;
                _course.InstructorEmail = txtInstructorEmail.Text;
                _course.InstructorPhone = txtInstructorPhone.Text;
                _course.Notes = txtNotes.Text;
                _course.GetNotified = pickerNotifications.SelectedIndex;
                _course.Term = _term.Id;
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.Update(_course);
                    await Navigation.PopAsync();
                }
            }
            else
            {
                await Navigation.PushModalAsync(new InputError());
            }

        }

        public async Task ShareCourseNotes()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = txtNotes.Text,
                Title = "Share Course Notes"
            });
        }

        private async void btnShareNotes_Clicked(object sender, EventArgs e)
        {
            await ShareCourseNotes();
        }

        private void btnDiscardChanges_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void btnViewAssessments_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AssessmentPage(_course, _main));
        }

        private bool IsUserInputValid()
        {
            bool valid = true;

            if (String.IsNullOrEmpty(txtCourseTitle.Text) ||
                pickerCourseStatus.SelectedItem == null ||
                dpStartDate.Date == null ||
                dpEndDate.Date == null ||
                dpEndDate.Date < dpStartDate.Date ||
                String.IsNullOrEmpty(txtInstructorName.Text) ||
                String.IsNullOrEmpty(txtInstructorEmail.Text) ||
                String.IsNullOrEmpty(txtInstructorPhone.Text) ||
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

        private async void btnDeleteCourse_Clicked(object sender, EventArgs e)
        {
            // delete assessments, then delete course
            var result = await this.DisplayAlert("Alert!", "Do you really want to delete this course?", "Yes", "Cancel");
            if (result)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    var assessments = conn.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}'");

                    foreach (Assessment assessment in assessments)
                    {
                        conn.Delete(assessment);
                    }
                    conn.Delete(_course);

                    await Navigation.PopAsync();
                }

            }
        }
    }
}