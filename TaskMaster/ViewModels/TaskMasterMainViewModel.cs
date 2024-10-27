
using CommunityToolkit.Mvvm.ComponentModel;
using Entities.Models;
using System.Reflection;
using TaskMaster.ViewsModels;

namespace TaskMaster.ViewModels
{
	public class TaskMasterMainViewModel: ObservableObject
	{
		public string Version { get; set; }

		public TaskTypesListViewModel TaskTypesListVM { get; set; }

		public TasksListViewModel TasksListVM { get; set; }

		public TaskMasterMainViewModel() 
		{
			Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			TaskTypesListVM = new TaskTypesListViewModel(new DragDropData());
			TasksListVM = new TasksListViewModel();
		}
	}
}
