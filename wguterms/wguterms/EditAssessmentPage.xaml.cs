using wguterms.Classes;
using SQLite;

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace wguterms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAssessmentPage : ContentPage
    {
        public Course _course;
        public Assessment _assessment;
        public MainPage _main;
        public List<string> pickerStates = new List<string> { "Objective", "Performance" };
        public List<string> pickerNotificationsStates = new List<string> { "Yes", "No" };
        public EditAssessmentPage(Course course, MainPage main, Assessment assessment)
        {
            _course = course;
            _assessment = assessment;
            _main = main;
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            pickerAssessmentType.ItemsSource = pickerStates;
            pickerAssessmentType.SelectedIndex = pickerStates.FindIndex(status => status == _assessment.AssessType);
            txtAssessmentName.Text = _assessment.AssessmentName;
            dpDueDate.Date = _assessment.End.Date;
            if (_assessment.GetNotified == 0)
            {
                pickerNotifications.SelectedIndex = 0;
            }
            else
            {
                pickerNotifications.SelectedIndex = 1;
            }
            base.OnAppearing();
        }

        private void btnDiscardChanges_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void btnEditCourse_Clicked(object sender, EventArgs e)
        {
            bool changedAssessmentType = false;
            if (_assessment.AssessType.ToString() != pickerAssessmentType.SelectedItem.ToString())
            {
                changedAssessmentType = true;
            }

            _assessment.AssessmentName = txtAssessmentName.Text;
            _assessment.End = dpDueDate.Date;
            _assessment.GetNotified = pickerNotifications.SelectedIndex;

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var objectiveCount = conn.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Objective'");
                var performanceCount = conn.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Performance'");

                var isAssemntPerformance = _assessment.AssessType.ToString() == "Performance";
                var isAssemntObjective = _assessment.AssessType.ToString() == "Objective";

                if (isAssemntObjective && objectiveCount.Count == 0)
                {
                    _assessment.AssessType = pickerAssessmentType.SelectedItem.ToString();
                    conn.Update(_assessment);
                    await Navigation.PopAsync();
                }
                else if (isAssemntPerformance && performanceCount.Count == 0)
                {
                    _assessment.AssessType = pickerAssessmentType.SelectedItem.ToString();
                    conn.Update(_assessment);
                    await Navigation.PopAsync();
                }
                else if (((isAssemntPerformance  && performanceCount.Count == 1) ||
                         (isAssemntObjective && objectiveCount.Count == 1)) &&
                         !(String.IsNullOrEmpty(_assessment.Id.ToString())) &&
                          !changedAssessmentType)
                {
                    conn.Update(_assessment);
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Alert", "You cannot have more than 1 Objective and 1 Performance Assessment for each course!", "OK");

                }
            }


        }

        private async void btnDeleteAssess_Clicked(object sender, EventArgs e)
        {
            // delete an assessment
            var result = await this.DisplayAlert("Alert!", "Do you really want to delete this assessment?", "Yes", "No");
            if (result)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.Delete(_assessment);
                    await Navigation.PopAsync();
                }

            }
        }
    }
}
