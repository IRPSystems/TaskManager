
using CommunityToolkit.Mvvm.ComponentModel;
using Entities.Models;
using System.Reflection;
using TaskMaster.ViewsModels;

namespace TaskMaster.ViewModels
{
	public class TaskMasterMainViewModel: ObservableObject
	{
		public string Version { get; set; }

		public TaskTypesListViewModel TasksListVM { get; set; }

		public TaskMasterMainViewModel() 
		{
			Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			TasksListVM = new TaskTypesListViewModel(new DragDropData());
		}
	}
}
