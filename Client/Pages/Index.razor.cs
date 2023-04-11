using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using TaskManager.Shared;
namespace TaskManager.Client.Pages
{
    public partial class Index
    {
        [Inject] public HttpClient Http { get; set; } = default!;
        private IList<TaskItem>? tasks = default!;
        private string? error;
        private string? newTask;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                string requestUri = "TaskItems";
                tasks = await
                    Http.GetFromJsonAsync<IList<TaskItem>>
                    (requestUri);
            }
            catch (Exception)
            {
                error = "Error Encountered";
            };
        }

        private async Task CheckboxChecked(TaskItem task)
        {
            task.IsComplete = !task.IsComplete;

            string requestUri = $"TaskItems/{task.TaskItemId}";
            var response = await
                Http.PutAsJsonAsync<TaskItem>(requestUri, task);
            if (!response.IsSuccessStatusCode)
            {
                error = response.ReasonPhrase;
            };
        }
        private async Task DeleteTask(TaskItem taskItem)
        {
            tasks.Remove(taskItem);

            string requestUri =
                 $"TaskItems/{taskItem.TaskItemId}";
            var response = await Http.DeleteAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                error = response.ReasonPhrase;
            };
        }
        private async Task EditTask(TaskItem taskItem)
        {
            Console.WriteLine(taskItem.TaskItemId);
            Console.WriteLine(taskItem.TaskName);
            TaskItem editTask = tasks.FirstOrDefault(p => p.TaskItemId == taskItem.TaskItemId);
            tasks.Remove(editTask);
            newTask = taskItem.TaskName;
            
                Console.WriteLine(tasks.FirstOrDefault(p => p.TaskItemId == taskItem.TaskItemId));
                Console.WriteLine(editTask.TaskName);
            
        }

        private async Task AddTask()
        {
            if (!string.IsNullOrWhiteSpace(newTask))
            {
                TaskItem newTaskItem = new TaskItem
                {
                    TaskName = newTask,
                    IsComplete = false
                };
                tasks.Add(newTaskItem);
                string requestUri = "TaskItems";
                var response = await
                Http.PostAsJsonAsync(requestUri,
                newTaskItem);
                if (response.IsSuccessStatusCode)
                {
                    newTask = string.Empty;
                    var task =
                    await response.Content.ReadFromJsonAsync
                    <TaskItem>();
                }
                else
                {
                    error = response.ReasonPhrase;
                };
            };
        }
    }
}


