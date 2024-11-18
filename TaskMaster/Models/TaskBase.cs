
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using TaskMaster.Enums;

namespace TaskMaster.Models
{
	public class TaskBase: ObservableObject, ICloneable
	{
		public TaskTypesEnum TaskType { get; set; }
		public virtual string TaskTypeName { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		[JsonIgnore]
		public bool IsExpanded { get; set; }
		[JsonIgnore]
		public bool IsSelected { get; set; }

		public TaskBase() 
		{
			IsExpanded = true;
			IsSelected = false;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
