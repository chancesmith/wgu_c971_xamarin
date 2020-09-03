using wguterms.Classes;
using SQLite;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace wguterms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentPage : ContentPage
    {
        public Course _course;
        public MainPage _main;
        public AssessmentPage(Course course, MainPage main)
        {
            _course = course;
            _main = main;
            InitializeComponent();

            AssessmentsListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            Title = course.CourseName;
        }

        protected override void OnAppearing()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Assessment>();
                var assessmentsForCourse = conn.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}'");

                AssessmentsListView.ItemsSource = assessmentsForCourse;
            }
        }


        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Assessment assessment = (Assessment)e.Item;
            await Navigation.PushAsync(new EditAssessmentPage(_course, _main, assessment));
        }

        async private void btnNewAssessment_Clicked(object sender, EventArgs e)
        {
            // only 2 assessments per course (1 perf & 1 obj)
            if (getAssessmentCount() < 2)
            {
                await Navigation.PushModalAsync(new AddAssessmentPage(_course, _main));
            }
            else
            {
                await DisplayAlert("Alert", "You cannot have more than 1 Objective and 1 Performance Assessment for each course!", "OK");
            }
        }

        int getAssessmentCount()
        {
            int count = 0;
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var assessmentCount = conn.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{_course.Id}'");
                count = assessmentCount.Count;
            }

            return count;
        }
    }
}
