
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeviceHandler.Models.DeviceFullDataModels;
using DeviceHandler.Models;
using ScriptHandler.ViewModels;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using TaskMaster.Models;

namespace TaskMaster.ViewModels
{
	public class TaskMasterMainViewModel: ObservableObject
	{
		#region Properties

		public string Version { get; set; }
		public ReleaseTaskMasterUserData ReleaseTaskMasterUserData { get; set; }

		public DesignViewModel Design { get; set; }
		public DockingViewModel Docking { get; set; }


		#endregion Properties

		#region Constructor

		public TaskMasterMainViewModel() 
		{
			Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			ChangeDarkLightCommand = new RelayCommand(ChangeDarkLight);
			ClosingCommand = new RelayCommand<CancelEventArgs>(Closing);

			ReleaseTaskMasterUserData = ReleaseTaskMasterUserData.LoadTaskMasterUserData("ReleaseTaskMaster");
			if (ReleaseTaskMasterUserData == null)
			{
				ReleaseTaskMasterUserData = new ReleaseTaskMasterUserData();
				ReleaseTaskMasterUserData.IsLightTheme = false;
				ChangeDarkLight();
				return;
			}
			else
			{
				ReleaseTaskMasterUserData.IsLightTheme = !ReleaseTaskMasterUserData.IsLightTheme;
			}

			ChangeDarkLight();

			Design = new DesignViewModel(null, null, ReleaseTaskMasterUserData.ScriptUserData, "ReleaseTasks", false);

			Docking = new DockingViewModel(Design);
		}

		#endregion Constructor

		#region Methods

		private void ChangeDarkLight()
		{
			ReleaseTaskMasterUserData.IsLightTheme = !ReleaseTaskMasterUserData.IsLightTheme;

			if (Application.Current != null)
				App.ChangeDarkLight(ReleaseTaskMasterUserData.IsLightTheme);
		}

		private void Closing(CancelEventArgs e)
		{
			SaveReleaseTaskMasterUserData();

			if (Design != null)
			{
				bool isCancel = Design.SaveIfNeeded();
				if (isCancel)
				{
					e.Cancel = true;
					return;
				}
			}

		}

		private void SaveReleaseTaskMasterUserData()
		{
			ReleaseTaskMasterUserData.SaveTaskMasterUserData(
				"ReleaseTaskMaster",
				ReleaseTaskMasterUserData);
		}

		#endregion Methods

		#region Commands

		public RelayCommand ChangeDarkLightCommand { get; private set; }
		public RelayCommand<CancelEventArgs> ClosingCommand { get; private set; }

		#endregion Commands
	}
}
