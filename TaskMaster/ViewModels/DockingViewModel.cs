
using Controls.ViewModels;
using ScriptHandler.ViewModels;
using ScriptHandler.Views;
using System.Windows.Controls;

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
			DesignViewModel designViewModel) :
			base("DockingReleaseTask")
		{
			CreateWindows(designViewModel);
		}

		#endregion Constructor

		#region Methods

		private void CreateWindows(DesignViewModel designViewModel)
		{
			DockFill = true;

			DesignView designView = new DesignView() { DataContext = designViewModel };
			CreateTabbedWindow(designView, "Design", string.Empty, out _design);
			SetDesiredWidthInDockedMode(_design, 1200);
		}

		#endregion Methods
	}
}
