
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScriptHandler.Models;
using ScriptHandler.Services;
using ScriptRunner.Services;
using ScriptRunner.ViewModels;

namespace TaskMaster.ViewModels
{
	public class RunViewModel: ObservableObject
	{
		#region Properties

		public RunExplorerViewModel RunExplorer { get; set; }
		public RunScriptService RunScript { get; set; }

		public string ErrorMessage { get; set; }

		public bool IsPlayEnabled
		{
			get => _isConnected && _isPlayEnabled && _isScriptsLoaded;
		}

		public bool IsPlayNotEnabled
		{
			get => !_isPlayEnabled && _isGeneralPlayEnabled;
		}


		#endregion Properties

		#region Fields

		private bool _isConnected;
		private bool _isPlayEnabled;
		private bool _isGeneralPlayEnabled;
		private bool _isGeneralEnabled;
		private bool _isScriptsLoaded;

		#endregion Fields

		#region Constructor

		public RunViewModel(ScriptUserData scriptUserData) 
		{
			StopScriptStepService stopScriptStep = new StopScriptStepService();
			RunScript = new RunScriptService(
				null,
				stopScriptStep,
				null,
				null);

			RunExplorer = new RunExplorerViewModel(
				null, 
				null, 
				RunScript, 
				scriptUserData);
		}

		#endregion Constructor

		#region Commands

		public RelayCommand StartAllCommand { get; private set; }
		public RelayCommand ForewardCommand { get; private set; }
		public RelayCommand AbortCommand { get; private set; }

		#endregion Commands
	}
}
