using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace test_oodrive2
{
    public partial class MainPage : ContentPage
    {
        private TaskDbContext _dbContext;
        private TaskModel _selectedTask;
        private ObservableCollection<TaskModel> _tasks;

        public MainPage()
        {
            InitializeComponent();
            _dbContext = new TaskDbContext();
            _tasks = new ObservableCollection<TaskModel>();
            TaskListView.ItemsSource = _tasks;
            LoadTasks();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _dbContext.Database.EnsureCreated();
            LoadTasks();  
        }

        private void LoadTasks()
        {
            _tasks.Clear();
            var tasks = _dbContext.Tasks.ToList();
            foreach (var task in tasks)
            {
                _tasks.Add(task);
            }
            UpdateTaskCounts(); 
        }

        private void UpdateTaskCounts()
        {
            int resolvedCount = _tasks.Count(t => t.IsCompleted);
            int notFinishedCount = _tasks.Count(t => !t.IsCompleted);
            ResolvedCountLabel.Text = $"Tasks Resolved: {resolvedCount}";
            NotFinishedCountLabel.Text = $"Tasks Not Finished: {notFinishedCount}";
        }

        public void AddTask(string taskName)
        {
            var newTask = new TaskModel
            {
                Name = taskName,
                CreatedDate = DateTime.Now,
                IsCompleted = false
            };

            _dbContext.Tasks.Add(newTask);
            _dbContext.SaveChanges();
            _tasks.Add(newTask); 
            UpdateTaskCounts(); 
        }

        private void OnAddTaskClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TaskEntry.Text))
            {
                AddTask(TaskEntry.Text);
                TaskEntry.Text = string.Empty;
            }
        }

        public void UpdateTask(int taskId, string newName, bool isCompleted)
        {
            var task = _dbContext.Tasks.Find(taskId);
            if (task != null)
            {
                task.Name = newName;
                task.IsCompleted = isCompleted;
                task.EndDate = isCompleted ? (DateTime?)DateTime.Now : null;
                _dbContext.SaveChanges();

               
                var taskInObservable = _tasks.FirstOrDefault(t => t.Id == taskId);
                if (taskInObservable != null)
                {
                    taskInObservable.Name = newName;
                    taskInObservable.IsCompleted = isCompleted;
                    taskInObservable.EndDate = task.EndDate;
                }
                UpdateTaskCounts(); 
            }
        }

        private void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var task = checkbox?.BindingContext as TaskModel;

            if (task != null)
            {
                UpdateTask(task.Id, task.Name, e.Value);
            }
        }

        public void DeleteTask(int taskId)
        {
            var task = _dbContext.Tasks.Find(taskId);
            if (task != null)
            {
                _dbContext.Tasks.Remove(task);
                _dbContext.SaveChanges();
                _tasks.Remove(task); 
                UpdateTaskCounts(); 
            }
        }

        private void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var task = button?.BindingContext as TaskModel;

            if (task != null)
            {
                DeleteTask(task.Id);
            }
        }

        private void OnEditTaskClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var task = button?.BindingContext as TaskModel;

            if (task != null)
            {
                _selectedTask = task;
                EditTaskEntry.Text = task.Name;
                EditTaskEntry.IsVisible = true;
                SaveChangesButton.IsVisible = true;
            }
        }

        private void OnSaveChangesClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(EditTaskEntry.Text) && _selectedTask != null)
            {
                UpdateTask(_selectedTask.Id, EditTaskEntry.Text, _selectedTask.IsCompleted);
                EditTaskEntry.Text = string.Empty;
                SaveChangesButton.IsVisible = false;
                EditTaskEntry.IsVisible = false;
                _selectedTask = null;
                TaskListView.SelectedItem = null;
                LoadTasks(); 
            }
        }
        private async void OnAnalyticsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AnalysisPage());
        }


        private void OnTaskSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is TaskModel task)
            {
                _selectedTask = task;
                EditTaskEntry.Text = task.Name;
                EditTaskEntry.IsVisible = true;
                SaveChangesButton.IsVisible = true;
            }
        }
    }
}
