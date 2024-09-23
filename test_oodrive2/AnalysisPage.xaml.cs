using Microsoft.Maui.Controls;
using System;
using System.Linq;
using TaskManagerApp.Data;

namespace test_oodrive2
{
    public partial class AnalysisPage : ContentPage
    {
        public AnalysisPage()
        {
            InitializeComponent();
        }

        private void OnAnalyzeButtonClicked(object sender, EventArgs e)
        {
            DateTime startDate = StartDatePicker.Date;
            DateTime endDate = EndDatePicker.Date;

            using (var dbContext = new TaskDbContext())
            {
                var createdTasks = dbContext.Tasks
                    .Where(task => task.CreatedDate >= startDate && task.CreatedDate <= endDate)
                    .ToList();

                var completedTasks = createdTasks
                    .Where(task => task.IsCompleted && task.EndDate.HasValue)
                    .ToList();

                CreatedTaskLabel.Text = $"Tâches créées : {createdTasks.Count}";
                CompletedTaskLabel.Text = $"Tâches complétées : {completedTasks.Count}";
            }
        }
    }
}
