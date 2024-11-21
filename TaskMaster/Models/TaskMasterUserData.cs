
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using ScriptHandler.Models;
using System.IO;

namespace TaskMaster.Models
{
	public class TaskMasterUserData: ObservableObject
	{
		public bool IsLightTheme { get; set; }

		public ScriptUserData ScriptUserData { get; set; }

		public TaskMasterUserData()
		{
			ScriptUserData = new ScriptUserData();
		}

		public static TaskMasterUserData LoadTaskMasterUserData(string dirName)
		{

			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			path = Path.Combine(path, dirName);
			if (Directory.Exists(path) == false)
			{
				return new TaskMasterUserData();
			}
			path = Path.Combine(path, "TaskMasterUserData.json");
			if (File.Exists(path) == false)
			{
				return new TaskMasterUserData();
			}


			string jsonString = File.ReadAllText(path);
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.Formatting = Formatting.Indented;
			settings.TypeNameHandling = TypeNameHandling.All;
			TaskMasterUserData askMasterUserData = JsonConvert.DeserializeObject(jsonString, settings) as TaskMasterUserData;
			if (askMasterUserData == null)
				return askMasterUserData;
			
			if(askMasterUserData.ScriptUserData == null)
				askMasterUserData.ScriptUserData = new ScriptHandler.Models.ScriptUserData();

			

			return askMasterUserData;
		}



		public static void SaveTaskMasterUserData(
			string dirName,
			TaskMasterUserData taskMasterUserData)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			path = Path.Combine(path, dirName);
			if (Directory.Exists(path) == false)
				Directory.CreateDirectory(path);
			path = Path.Combine(path, "TaskMasterUserData.json");

			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.Formatting = Formatting.Indented;
			settings.TypeNameHandling = TypeNameHandling.All;
			var sz = JsonConvert.SerializeObject(taskMasterUserData, settings);
			File.WriteAllText(path, sz);
		}
	}
}
