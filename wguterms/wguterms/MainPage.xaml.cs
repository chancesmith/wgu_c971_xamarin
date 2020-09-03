using wguterms.Classes;
using SQLite;

using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

using Plugin.LocalNotifications;

namespace wguterms
{
    public partial class MainPage : ContentPage
    {

        public List<Term> terms = new List<Term>();
        public List<Course> courses = new List<Course>();
        public List<Assessment> assessments = new List<Assessment>();
        public MainPage main;

        bool isInitRound = true;

        public MainPage()
        {
            InitializeComponent();
            termsListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            main = this;
        }

        protected override void OnAppearing()
        {

            
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Term>();
                conn.CreateTable<Course>();
                conn.CreateTable<Assessment>();

                terms = conn.Table<Term>().ToList();
            }
            if (terms.Any() && isInitRound)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.DropTable<Assessment>();
                    conn.DropTable<Course>();
                    conn.DropTable<Term>();

                    conn.CreateTable<Term>();
                    conn.CreateTable<Course>();
                    conn.CreateTable<Assessment>();

                    SeedAppWithData(1);
                }
                isInitRound = false;
                RunAlerts();
            }
            else if (isInitRound)
            {

                SeedAppWithData(1);

                isInitRound = false;
                RunAlerts();
            }
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                terms = conn.Table<Term>().ToList();
                termsListView.ItemsSource = terms;
            }

            base.OnAppearing();
        }

        private void RunAlerts()
        {
            foreach (Term t in terms)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    var courses = conn.Query<Course>($"SELECT * FROM Courses WHERE Term = '{t.Id}'");
                    foreach (Course course in courses)
                    {
                        var isCourseStartingInThreeDays = (course.Start - DateTime.Now).TotalDays < 3;
                        if (isCourseStartingInThreeDays && course.GetNotified == 1)
                        {
                            CrossLocalNotifications.Current.Show("New Course Starting:", $"{course.CourseName} is starting on {course.Start.Date}");
                        }

                        var isCourseEndingInSevenDays = (course.End - DateTime.Now).TotalDays < 7;
                        if (isCourseEndingInSevenDays && course.GetNotified == 1)
                        {
                            CrossLocalNotifications.Current.Show("Current Course Ending: ", $"{course.CourseName} is ending on {course.End.Date}");
                        }

                        var assessments = conn.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{course.Id}'");
                        foreach (Assessment a in assessments)
                        {
                            var isAssmntStartingInThreeDays = (a.End - DateTime.Now).TotalDays < 3;
                            if (isAssmntStartingInThreeDays && a.GetNotified == 1)
                            {
                                CrossLocalNotifications.Current.Show("Assessment Due: ", $"{a.AssessmentName} is starting on {a.End.Date}");
                            }
                        }

                    }
                }
            }
        }
        private void SeedAppWithData(int termNumber)
        {
            //-// seed term
            Term newTerm = new Term();
            newTerm.TermName = "Term " + termNumber.ToString();
            newTerm.Start = new DateTime(2020, 09, 15);
            newTerm.End = new DateTime(2020, 12, 15);
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.Insert(newTerm);
            }

            //-// seed course
            Course newCourse = new Course();
            newCourse.Term = newTerm.Id;
            newCourse.CourseName = "Intro To Programming";
            newCourse.CourseStatus = "Plan To Take";
            newCourse.Start = new DateTime(2020, 09, 23);
            newCourse.End = new DateTime(2020, 10, 10);
            newCourse.InstructorName = "Sohn Smith";
            newCourse.InstructorEmail = "jsmith@wgu.edu";
            newCourse.InstructorPhone = "777-555-1234";
            newCourse.Notes = "So close to the end.";
            newCourse.GetNotified = 1;
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.Insert(newCourse);
            }

            //-// seed obj assessment
            Assessment newObjectiveAssessment = new Assessment();
            newObjectiveAssessment.AssessmentName = "FOO1";
            newObjectiveAssessment.Start = new DateTime(2020, 09, 11);
            newObjectiveAssessment.End = new DateTime(2020, 09, 11);
            newObjectiveAssessment.AssessType = "Objective";
            newObjectiveAssessment.Course = newCourse.Id;
            newObjectiveAssessment.GetNotified = 0;
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.Insert(newObjectiveAssessment);
            }

            //-// seed perf assesssment
            Assessment newPerformanceAssessment = new Assessment();
            newPerformanceAssessment.AssessmentName = "BAR2";
            newPerformanceAssessment.Start = new DateTime(2020, 09, 11);
            newPerformanceAssessment.End = new DateTime(2020, 09, 11);
            newPerformanceAssessment.AssessType = "Performance";
            newPerformanceAssessment.Course = newCourse.Id;
            newPerformanceAssessment.GetNotified = 0;
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.Insert(newPerformanceAssessment);
            }
        }

        async private void btnNewTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddTerm(this));
        }

        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Term term = (Term)e.Item;
            await Navigation.PushAsync(new TermPage(term, main));
        }
    }
}