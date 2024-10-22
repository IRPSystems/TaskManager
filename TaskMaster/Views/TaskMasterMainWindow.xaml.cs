using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskMaster.ViewModels;

namespace TaskMaster.Views

{
	/// <summary>
	/// Interaction logic for TaskMasterMainWindow.xaml
	/// </summary>
	public partial class TaskMasterMainWindow : Window
	{
		public TaskMasterMainWindow()
		{
			InitializeComponent();

			DataContext = new TaskMasterMainViewModel();
		}
	}
}