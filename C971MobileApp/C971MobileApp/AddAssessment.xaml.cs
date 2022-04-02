using C971MobileApp.Models;
using C971MobileApp.Services;
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971MobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssessment : ContentPage
    {
        private SQLiteAsyncConnection _conn;
        private Course _course;
        public AddAssessment(Course course)
        {
            InitializeComponent();
            _course = course;
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            await _conn.CreateTableAsync<Assessment>();
            var objectiveCount = await _conn.QueryAsync<Assessment>($"Select Type From Assessments Where Course = '{_course.Id}' And Type = 'Objective'");
            var performanceCount = await _conn.QueryAsync<Assessment>($"Select Type From Assessments Where Course = '{_course.Id}' And Type = 'Performance'");
            if (objectiveCount.Count == 0)
            {
                AssessmentType.Items.Add("Objective");
            }
            if (performanceCount.Count == 0)
            {
                AssessmentType.Items.Add("Performance");
            }
            if (objectiveCount.Count == 1)
            {
                AssessmentType.Items.Remove("Objective");
            }
            if (performanceCount.Count == 1)
            {
                AssessmentType.Items.Remove("Performance");
            }
            base.OnAppearing();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Add_Assessment_Clicked(object sender, EventArgs e)
        {
            // Create a new assessment
            var assessment = new Assessment();
            assessment.Title = AssessmentName.Text;
            assessment.StartDate = StartDate.Date;
            assessment.EndDate = EndDate.Date;
            assessment.Course = _course.Id;
            assessment.Type = (string)AssessmentType.SelectedItem;
            assessment.NotificationEnabled = EnableNotifications.On == true ? 1 : 0;

            if (FieldCheck.IsNull(AssessmentName.Text))
            {
                if (assessment.StartDate <= assessment.EndDate)
                {
                    await _conn.InsertAsync(assessment);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error.", "Please ensure start date is before end date.", "Ok");
            }
            else await DisplayAlert("Error.", "Please ensure all fields are completed.", "Ok");
        }
    }
}