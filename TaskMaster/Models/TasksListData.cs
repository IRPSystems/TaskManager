
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TaskMaster.Models
{
    public class TasksListData: ObservableObject
    {
		public ObservableCollection<TaskBase> TasksList { get; set; }
		public string TasksListName { get; set; }
		public string TasksListPath { get; set; }

		public TasksListData()
		{
			TasksList = new ObservableCollection<TaskBase>();
		}
	}
}
