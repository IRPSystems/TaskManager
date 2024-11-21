
using Controls.ViewModels;
using ScriptHandler.ViewModels;
using ScriptHandler.Views;
using System.Windows.Controls;
using TaskMaster.Views;

namespace TaskMaster.ViewModels
{
	public class DockingViewModel : DocingBaseViewModel
	{
		#region Fields

		private ContentControl _design;
		private ContentControl _run;

		#endregion Fields

		#region Constructor

		public DockingViewModel(
			DesignViewModel designViewModel,
			RunViewModel run) :
			base("DockingReleaseTask")
		{
			CreateWindows(designViewModel, run);
		}

		#endregion Constructor

		#region Methods

		private void CreateWindows(
			DesignViewModel designViewModel,
			RunViewModel run)
		{
			DockFill = true;

			DesignView designView = new DesignView() { DataContext = designViewModel };
			CreateTabbedWindow(designView, "Design", string.Empty, out _design);
			SetDesiredWidthInDockedMode(_design, 1200);

			RunView runView = new RunView() { DataContext = run };
			CreateTabbedWindow(runView, "Run", "Design", out _run);
		}

		#endregion Methods
	}
}
