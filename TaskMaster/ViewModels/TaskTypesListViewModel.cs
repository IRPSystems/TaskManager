
using CommunityToolkit.Mvvm.ComponentModel;
using Services.Services;
using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using TaskMaster.Models;
using Entities.Models;

namespace TaskMaster.ViewsModels
{
	public class TaskTypesListViewModel : ObservableObject
	{
		#region Properties

		public ObservableCollection<TaskBase> TaskList { get; set; }

		#endregion Properties

		#region Fields

		private DragDropData _designDragDropData;

		#endregion Fields

		#region Constructor

		public TaskTypesListViewModel(
			DragDropData designDragDropData)
		{
			_designDragDropData = designDragDropData;
			InitTaskList();
		}

		#endregion Constructor

		#region Methods

		private void InitTaskList()
		{
			LoggerService.Inforamtion(this, "Initiating the tools list");

			Assembly assembly = Assembly.GetExecutingAssembly();

			List<Type> typesList = assembly.GetTypes().ToList();
			typesList = typesList.Where((t) => t.Namespace == "TaskMaster.Models.Tasks").ToList();

			TaskList = new ObservableCollection<TaskBase>();
			foreach (Type type in typesList)
			{
				if (!IsNodeBase(type))
					continue;


				var c = Activator.CreateInstance(type);
				TaskList.Add(c as TaskBase);
			}
		}

		private bool IsNodeBase(Type type)
		{
			while(type.BaseType.Name != "TaskBase")
			{
				if (type.BaseType.Name == "Object")
					return false;

				type = type.BaseType;
			}

			return true;
		}


		#region Drag

		private void TaskList_MouseEnter(MouseEventArgs e)
		{
			if (_designDragDropData.IsIgnor)
				return;

			if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
				_designDragDropData.IsMouseDown = true;
			else
				_designDragDropData.IsMouseDown = false;
		}

		private void TaskList_PreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (_designDragDropData.IsIgnor)
				return;

			_designDragDropData.IsMouseDown = true;
			_designDragDropData.StartPoint = e.GetPosition(null);
		}

		private void TaskList_PreviewMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			_designDragDropData.IsMouseDown = false;
		}

		private void TaskList_MouseMove(MouseEventArgs e)
		{
			if (_designDragDropData.IsMouseDown == false)
				return;

			DragObject(e);
		}

		private void DragObject(MouseEventArgs e)
		{
			LoggerService.Inforamtion(this, "Object is draged");

			Point mousePos = e.GetPosition(null);
			Vector diff = _designDragDropData.StartPoint - mousePos;

			if (e.LeftButton == MouseButtonState.Pressed &&
				Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				string formate = "TaskTypeToList";
				

				// Get the dragged ListViewItem
				ListView listView =
					FindAncestorService.FindAncestor<ListView>((DependencyObject)e.OriginalSource);
				ListViewItem listViewItem =
					FindAncestorService.FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

				DependencyObject sourceObject = listViewItem;

				object item = null;
				if (listView != null && listViewItem != null)
				{
					// Find the data behind the ListViewItem
					item = listView.ItemContainerGenerator.
						ItemFromContainer(listViewItem);
				}

				if (item == null)
					return;
				

				DataObject dragData = new DataObject(formate, item);
				DragDrop.DoDragDrop(sourceObject, dragData, DragDropEffects.Move);
			}
		}

		

		#endregion Drag

		private void TaskList_MouseDoubleClick(MouseButtonEventArgs e)
		{
			if (!(e.Source is ListView listView))
				return;

			if (listView.SelectedItem == null)
				return;

			if (!(listView.SelectedItem is TaskBase scriptNode))
				return;

			AddNodeEvent?.Invoke(scriptNode);
		}

		#endregion Methods

		#region Commands

		#region Drag

		private RelayCommand<MouseEventArgs> _TaskList_MouseEnterCommand;
		public RelayCommand<MouseEventArgs> TaskList_MouseEnterCommand
		{
			get
			{
				return _TaskList_MouseEnterCommand ?? (_TaskList_MouseEnterCommand =
					new RelayCommand<MouseEventArgs>(TaskList_MouseEnter));
			}
		}

		private RelayCommand<MouseButtonEventArgs> _TaskList_PreviewMouseLeftButtonDownCommant;
		public RelayCommand<MouseButtonEventArgs> TaskList_PreviewMouseLeftButtonDownCommant
		{
			get
			{
				return _TaskList_PreviewMouseLeftButtonDownCommant ?? (_TaskList_PreviewMouseLeftButtonDownCommant =
					new RelayCommand<MouseButtonEventArgs>(TaskList_PreviewMouseLeftButtonDown));
			}
		}

		private RelayCommand<MouseButtonEventArgs> _TaskList_PreviewMouseLeftButtonUpCommant;
		public RelayCommand<MouseButtonEventArgs> TaskList_PreviewMouseLeftButtonUpCommant
		{
			get
			{
				return _TaskList_PreviewMouseLeftButtonUpCommant ?? (_TaskList_PreviewMouseLeftButtonUpCommant =
					new RelayCommand<MouseButtonEventArgs>(TaskList_PreviewMouseLeftButtonUp));
			}
		}

		private RelayCommand<MouseEventArgs> _TaskList_MouseMoveCommand;
		public RelayCommand<MouseEventArgs> TaskList_MouseMoveCommand
		{
			get
			{
				return _TaskList_MouseMoveCommand ?? (_TaskList_MouseMoveCommand =
					new RelayCommand<MouseEventArgs>(TaskList_MouseMove));
			}
		}

		#endregion Drag


		private RelayCommand<MouseButtonEventArgs> _TaskList_MouseDoubleClickCommand;
		public RelayCommand<MouseButtonEventArgs> TaskList_MouseDoubleClickCommand
		{
			get
			{
				return _TaskList_MouseDoubleClickCommand ?? (_TaskList_MouseDoubleClickCommand =
					new RelayCommand<MouseButtonEventArgs>(TaskList_MouseDoubleClick));
			}
		}

		#endregion Commands

		#region Events

		public event Action<TaskBase> AddNodeEvent;

		#endregion Events
	}
}
