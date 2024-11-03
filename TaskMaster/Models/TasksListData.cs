
using System.Collections.ObjectModel;

namespace TaskMaster.Models
{
    public class TasksListData
    {
		public ObservableCollection<TaskBase> TasksList { get; set; }
		public string TasksListName { get; set; }
		public string TasksListPath { get; set; }
	}
}
