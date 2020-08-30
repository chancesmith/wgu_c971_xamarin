﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using wguterms.Classes;
using SQLite;
using Plugin.LocalNotifications;

namespace wguterms
{
    public partial class MainPage : ContentPage
    {

        public List<Term> terms = new List<Term>();
        public List<Course> courses = new List<Course>();
        public List<Assessment> assessments = new List<Assessment>();
        public MainPage main;

        bool isFirstPass = true;

        public MainPage()
        {
            InitializeComponent();
            termsListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(ItemTapped);
            main = this;
        }

        protected override void OnAppearing()
        {

            
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.CreateTable<Term>();
                con.CreateTable<Course>();
                con.CreateTable<Assessment>();

                terms = con.Table<Term>().ToList();
            }
            if (terms.Any() && isFirstPass)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    con.DropTable<Assessment>();
                    con.DropTable<Course>();
                    con.DropTable<Term>();

                    con.CreateTable<Term>();
                    con.CreateTable<Course>();
                    con.CreateTable<Assessment>();

                    CreateEvaluationData(1);
                }
                isFirstPass = false;
                RunAlerts();
            }
            else if (isFirstPass)
            {

                CreateEvaluationData(1);

                isFirstPass = false;
                RunAlerts();
            }
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                terms = con.Table<Term>().ToList();
                termsListView.ItemsSource = terms;
            }

            base.OnAppearing();
        }

        private void RunAlerts()
        {
            foreach (Term t in terms)
            {
                using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
                {
                    var courses = con.Query<Course>($"SELECT * FROM Courses WHERE Term = '{t.Id}'");
                    foreach (Course c in courses)
                    {
                        // Check for courses starting within 3 days
                        if ((c.Start - DateTime.Now).TotalDays < 3 && c.GetNotified == 1)
                        {
                            CrossLocalNotifications.Current.Show("Course Starting Soon", $"{c.CourseName} is starting on {c.Start.Date.ToString()}");
                        }
                        // Check for courses ending within 7 days
                        if ((c.End - DateTime.Now).TotalDays < 7 && c.GetNotified == 1)
                        {
                            CrossLocalNotifications.Current.Show("Course Ending Soon", $"{c.CourseName} is ending on {c.End.Date.ToString()}");
                        }

                        //Check for assessments that are coming up within 3 days
                        var assessments = con.Query<Assessment>($"SELECT * FROM Assessments WHERE Course = '{c.Id}'");
                        foreach (Assessment a in assessments)
                        {
                            if ((a.End - DateTime.Now).TotalDays < 3 && a.GetNotified == 1)
                            {
                                CrossLocalNotifications.Current.Show("Assessment Due Soon", $"{a.AssessmentName} is starting on {a.End.Date.ToString()}");
                            }
                        }

                    }
                }
            }
        }
        private void CreateEvaluationData(int termNumber)
        {
            //// EVALUATION DATA CREATION
            //// SEED TERM----
            Term newTerm = new Term();
            newTerm.TermName = "Term " + termNumber.ToString();
            newTerm.Start = new DateTime(2020, 03, 14);
            newTerm.End = new DateTime(2020, 09, 14);
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(newTerm);
            }
            //// SEED COURSE----
            Course newCourse = new Course();
            newCourse.Term = newTerm.Id;
            newCourse.CourseName = "Intro To Theoretical Physics";
            newCourse.CourseStatus = "Plan To Take";
            newCourse.Start = new DateTime(2020, 03, 23);
            newCourse.End = new DateTime(2020, 06, 11);
            newCourse.InstructorName = "David Potesta";
            newCourse.InstructorEmail = "dpotest@wgu.edu";
            newCourse.InstructorPhone = "414-768-3782";
            newCourse.Notes = "Hurry Up";
            newCourse.GetNotified = 1;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(newCourse);
            }
            //// SEED OBJECTIVE ASSESSMENT----
            Assessment newObjectiveAssessment = new Assessment();
            newObjectiveAssessment.AssessmentName = "BOP1";
            newObjectiveAssessment.Start = new DateTime(2020, 04, 11);
            newObjectiveAssessment.End = new DateTime(2020, 04, 11);
            newObjectiveAssessment.AssessType = "Objective";
            newObjectiveAssessment.Course = newCourse.Id;
            newObjectiveAssessment.GetNotified = 0;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(newObjectiveAssessment);
            }
            //// SEED PERFORMANCE ASSESSMENT----
            Assessment newPerformanceAssessment = new Assessment();
            newPerformanceAssessment.AssessmentName = "LAG1";
            newPerformanceAssessment.Start = new DateTime(2020, 06, 11);
            newPerformanceAssessment.End = new DateTime(2020, 06, 11);
            newPerformanceAssessment.AssessType = "Performance";
            newPerformanceAssessment.Course = newCourse.Id;
            newPerformanceAssessment.GetNotified = 0;
            using (SQLiteConnection con = new SQLiteConnection(App.FilePath))
            {
                con.Insert(newPerformanceAssessment);
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