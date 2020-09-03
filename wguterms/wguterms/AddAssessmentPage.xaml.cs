using wguterms.Classes;
using SQLite;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace wguterms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssessmentPage : ContentPage
    {
        public Course _course;
        public MainPage _main;
        public AddAssessmentPage(Course course, MainPage main)
        {
            _course = course;
            _main = main;
            InitializeComponent();
        }

        private void btnDiscardChanges_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        async private void btnAddAssessment_Clicked(object sender, EventArgs e)
        {
            if (IsUserInputValid())
            {
                Assessment newAssessment = new Assessment();
                newAssessment.AssessmentName = txtAssessmentName.Text;
                newAssessment.AssessType = pickerAssessmentType.SelectedItem.ToString();
                newAssessment.End = dpDueDate.Date;
                newAssessment.GetNotified = pickerNotifications.SelectedIndex;
                newAssessment.Course = _course.Id;

                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    var objectiveCount = conn.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Objective'");
                    var performanceCount = conn.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}' AND AssessType = 'Performance'");
                    if (newAssessment.AssessType.ToString() == "Objective" && objectiveCount.Count == 0)
                    {
                        conn.Insert(newAssessment);
                        _main.assessments.Add(newAssessment);
                        await Navigation.PopModalAsync();
                    }
                    else if (newAssessment.AssessType.ToString() == "Performance" && performanceCount.Count == 0)
                    {
                        conn.Insert(newAssessment);
                        _main.assessments.Add(newAssessment);
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert("Alert", "You cannot have more than 1 Objective and 1 Performance Assessment for each course!", "OK");

                    }
                }
            }
            else
            {
                await Navigation.PushModalAsync(new InputError());
            }
        }

        private bool IsUserInputValid()
        {
            bool valid = true;

            if (txtAssessmentName == null ||
                pickerAssessmentType.SelectedItem == null ||
                dpDueDate.Date == null ||
                pickerNotifications.SelectedItem == null
                )

            {
                return false;
            }
            return valid;
        }
    }
}
