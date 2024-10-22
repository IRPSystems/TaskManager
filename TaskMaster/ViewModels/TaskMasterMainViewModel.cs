
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

namespace TaskMaster.ViewModels
{
	public class TaskMasterMainViewModel: ObservableObject
	{
		public string Version { get; set; }

		public TaskMasterMainViewModel() 
		{
			Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}
	}
}
