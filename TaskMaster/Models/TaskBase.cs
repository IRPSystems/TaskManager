
using CommunityToolkit.Mvvm.ComponentModel;
using TaskMaster.Enums;

namespace TaskMaster.Models
{
	public class TaskBase: ObservableObject
	{
		public TaskTypesEnum TaskType { get; set; }
		public virtual string TaskTypeName { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
