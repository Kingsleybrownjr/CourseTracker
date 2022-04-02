using C971MobileApp.Models;
using C971MobileApp.Services;
using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace C971MobileApp
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private SQLiteAsyncConnection _conn;
        public ObservableCollection<Term> _termList;
        private bool pushNotification = true;

        public MainPage()
        {
            InitializeComponent();
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
            termListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(Term_Click);
        }

        protected override async void OnAppearing()
        {
            await _conn.CreateTableAsync<Term>();
            await _conn.CreateTableAsync<Course>();
            await _conn.CreateTableAsync<Assessment>();

            var termList = await _conn.Table<Term>().ToListAsync();
            var courseList = await _conn.Table<Course>().ToListAsync();
            var assessmentList = await _conn.Table<Assessment>().ToListAsync();

            // Seed app with dummy data if no data exists.
            if (!termList.Any())
            {
                var dummyTerm = new Term();
                dummyTerm.Title = "Term 1";
                dummyTerm.StartDate = new DateTime(2022, 04, 01);
                dummyTerm.EndDate = new DateTime(2022, 10, 31);
                await _conn.InsertAsync(dummyTerm);
                termList.Add(dummyTerm);

                var dummyCourse = new Course();
                dummyCourse.CourseName = "C971 Mobile Application";
                dummyCourse.StartDate = new DateTime(2022, 04, 01);
                dummyCourse.EndDate = new DateTime(2022, 04, 30);
                dummyCourse.Status = "In-Progress";
                dummyCourse.InstructorName = "Kinsley Brown";
                dummyCourse.InstructorPhone = "713-385-1098";
                dummyCourse.InstructorEmail = "kbr1748@wgu.edu";
                dummyCourse.NotificationEnabled = 1;
                dummyCourse.Notes = "Such an interesting course!";
                dummyCourse.Term = dummyTerm.Id;
                await _conn.InsertAsync(dummyCourse);

                var dummyObjectiveAssessment = new Assessment();
                dummyObjectiveAssessment.Title = "Test Assessment 1";
                dummyObjectiveAssessment.StartDate = new DateTime(2022, 04, 01);
                dummyObjectiveAssessment.EndDate = new DateTime(2022, 04, 02);
                dummyObjectiveAssessment.Course = dummyCourse.Id;
                dummyObjectiveAssessment.Type = "Objective";
                dummyObjectiveAssessment.NotificationEnabled = 1;
                await _conn.InsertAsync(dummyObjectiveAssessment);

                var dummyPerformanceAssessment = new Assessment();
                dummyPerformanceAssessment.Title = "Test Assessment 2";
                dummyPerformanceAssessment.StartDate = new DateTime(2022, 04, 12);
                dummyPerformanceAssessment.EndDate = new DateTime(2022, 04, 30);
                dummyPerformanceAssessment.Course = dummyCourse.Id;
                dummyPerformanceAssessment.Type = "Performance";
                dummyPerformanceAssessment.NotificationEnabled = 1;
                await _conn.InsertAsync(dummyPerformanceAssessment);
            }

            // Handle Notification
            if (pushNotification == true)
            {
                pushNotification = false;
                int courseId = 0;
                foreach (Course course in courseList)
                {
                    courseId++;
                    if (course.NotificationEnabled == 1)
                    {
                        if (course.StartDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseName} begins today!", courseId);
                        if (course.EndDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseName} ends today!", courseId);
                    }
                }

                int assessmentId = courseId;
                foreach (Assessment assessment in assessmentList)
                {
                    assessmentId++;
                    if (assessment.NotificationEnabled == 1)
                    {
                        if (assessment.StartDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{assessment.Title} begins today!", assessmentId);
                        if (assessment.EndDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{assessment.Title} ends today!", assessmentId);
                    }
                }
            }
            _termList = new ObservableCollection<Term>(termList);
            termListView.ItemsSource = _termList;
            base.OnAppearing();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddTerm(this));
        }

        async private void Term_Click(object sender, ItemTappedEventArgs e)
        {
            Term term = (Term)e.Item;
            await Navigation.PushModalAsync(new TermDetail(term));
        }
    }
}
