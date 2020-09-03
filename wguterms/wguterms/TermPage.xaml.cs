using wguterms.Classes;
using SQLite;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace wguterms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermPage : ContentPage
    {
        public Term _term;
        public MainPage _main;

        public TermPage()
        {
            InitializeComponent();
        }

        public TermPage(Term term, MainPage main)
        {
            _term = term;
            _main = main;
            InitializeComponent();
            coursesListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            Title = term.TermName;


        }
        protected override void OnAppearing()
        {
            termStart.Text = _term.Start.ToString("MM/dd/yyyy");
            termEnd.Text = _term.End.ToString("MM/dd/yyyy");
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Course>();
                var coursesForTerm = conn.Query<Course>($"SELECT * FROM Courses WHERE Term = '{_term.Id}'");
                coursesListView.ItemsSource = coursesForTerm;
            }
        }

        async private void btnAddCourse_Clicked(object sender, EventArgs e)
        {
            // only 6 courses per/term
            if (GetCourseCount() < 6)
            {
                await Navigation.PushModalAsync(new AddCourse(_term, _main));
            }
            else
            {
                await DisplayAlert("Alert", "Can't add more than 6 courses", "OK");
            }
        }

        int GetCourseCount()
        {
            int count = 0;
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                var courseCount = conn.Query<Course>($"SELECT * FROM Courses WHERE Term = '{_term.Id}'");
                count = courseCount.Count;
            }

            return count;
        }

        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Course course = (Course)e.Item;
            await Navigation.PushAsync(new CoursePage(_term, _main, course));
        }

        private async void btnDeleteTerm_Clicked(object sender, EventArgs e)
        {
            // delete assessments, then course
            var result = await DisplayAlert("Alert!", "Ready to delete this term?", "Yes", "No");
            if (result)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    var courses = conn.Query<Course>($"SELECT * FROM Courses WHERE Term = '{_term.Id}'");
                    foreach (Course course in courses)
                    {
                        var assessments = conn.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{course.Id}'");
                        foreach (Assessment a in assessments)
                        {
                            conn.Delete(a);
                        }
                        conn.Delete(course);
                    }
                    conn.Delete(_term);
                    await Navigation.PopToRootAsync();
                }

            }
        }

        private async void btnEditTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditTermPage(_term, _main));
        }
    }
}