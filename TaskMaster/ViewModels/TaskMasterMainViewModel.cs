
using CommunityToolkit.Mvvm.ComponentModel;
using ScriptHandler.ViewModels;
using System.Reflection;
using TaskMaster.Models;

namespace TaskMaster.ViewModels
{
	public class TaskMasterMainViewModel: ObservableObject
	{
		public string Version { get; set; }
		public TaskMasterUserData TaskMasterUserData { get; set; }

		public DesignViewModel Design { get; set; }


		public TaskMasterMainViewModel() 
		{
			Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			TaskMasterUserData = TaskMasterUserData.LoadTaskMasterUserData("Evva");

			Design = new DesignViewModel(null, null, TaskMasterUserData.ScriptUserData, "Tasks", false);
		}
	}
}
