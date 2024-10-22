
using CommunityToolkit.Mvvm.ComponentModel;
using TaskMaster.Enums;

namespace TaskMaster.Models
{
	public class TaskBase: ObservableObject
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public TaskTypesEnum TaskType { get; set; }
	}
}
