using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using ScriptHandler.Views;
using Services.Services;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TaskMaster.Models;

namespace TaskMaster.ViewModels
{
	public class TasksListViewModel : ObservableObject
	{
		#region Properties

		public TasksListData TasksList { get; set; }


		private bool _isChanged;
		public bool IsChanged
		{
			get => _isChanged;
			set
			{

				//ScriptIsChangedEvent?.Invoke(this, value);
				_isChanged = value;
				OnPropertyChanged(nameof(IsChanged));
			}
		}


		#endregion Properties

		#region Fields

		private bool _isMouseDown;
		private Point _startPoint;


		private TaskBase _selectedTask;


		private IList _selectedItems;


		#endregion Fields

		#region Constructor

		public TasksListViewModel()
		{
			TasksList = new TasksListData()
			{
				TasksList = new ObservableCollection<TaskBase>()
			};

			MoveTaskUpCommand = new RelayCommand(MoveTaskUp);
			MoveTaskDownCommand = new RelayCommand(MoveTaskDown);
			DeleteCommand = new RelayCommand(Delete);

			CopyScriptTaskCommand = new RelayCommand(CopyScriptTask);

			ScriptExpandAllCommand = new RelayCommand(ScriptExpandAll);
			ScriptCollapseAllCommand = new RelayCommand(ScriptCollapseAll);

			CopyCommand = new RelayCommand(Copy);
			PastCommand = new RelayCommand(Past);

			//NewCommand = new RelayCommand(New);
			SaveCommand = new RelayCommand(Save);
			LoadCommand = new RelayCommand(Load);


			_isMouseDown = false;


			//_moveTaskService = new MoveTaskService();



			LoggerService.Inforamtion(this, "Finished init of Design");

			IsChanged = false;

		}

		#endregion Constructor

		#region Methods

		private void Loaded()
		{
			IsChanged = false;


		}

		#region Expand/Collapse


		private void ScriptExpandAll()
		{
			LoggerService.Inforamtion(this, "Expanding all the tasks");

			foreach (TaskBase taskBase in TasksList.TasksList)
				taskBase.IsExpanded = true;
		}

		private void ScriptCollapseAll()
		{
			LoggerService.Inforamtion(this, "Collapsing all the tasks");

			foreach (TaskBase taskBase in TasksList.TasksList)
				taskBase.IsExpanded = false;
		}

		#endregion Expand/Collapse

		#region Drag

		private void TaskList_MouseEnter(MouseEventArgs e)
		{

			if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
				_isMouseDown = true;
			else
				_isMouseDown = false;
		}

		private void TaskList_PreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{

			// If mouse down on the scroll bar, don't do anything.
			var thumb =
				FindAncestorService.FindAncestor<Thumb>((DependencyObject)e.OriginalSource);
			if (thumb != null)
				return;



			_isMouseDown = true;
			_startPoint = e.GetPosition(null);
		}

		private void TaskList_PreviewMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			_isMouseDown = false;
		}


		private void TaskList_MouseMove(MouseEventArgs e)
		{
			if (_isMouseDown == false)
				return;


			LoggerService.Inforamtion(this, "Object is draged");

			Point mousePos = e.GetPosition(null);
			Vector diff = _startPoint - mousePos;

			if (e.LeftButton == MouseButtonState.Pressed &&
				Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				string formate = "ChangeTaskPlace";

				// Get the dragged ListViewItem
				ListView listView =
					FindAncestorService.FindAncestor<ListView>((DependencyObject)e.OriginalSource);
				if (listView == null)
					return;

				ListViewItem listViewItem =
						FindAncestorService.FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
				if (listViewItem == null)
					return;

				if (!listView.SelectedItems.Contains(listViewItem.DataContext))
				{
					DataObject dragData = new DataObject(formate, listViewItem.DataContext);
					DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
				}
				else
				{
					DataObject dragData = new DataObject(formate, listView.SelectedItems);
					DragDrop.DoDragDrop(listView, dragData, DragDropEffects.Move);
				}




			}
		}

		private void TaskList_DragOver(DragEventArgs e)
		{
			if (!(e.Source is ListView li))
				return;
			//ListBox li = sender as ListBox;
			ScrollViewer sv = FindChildService.FindChild<ScrollViewer>(li);
			if (sv == null)
				return;

			double tolerance = 10;
			double verticalPos = e.GetPosition(li).Y;
			double offset = 3;

			if (verticalPos < tolerance) // Top of visible list?
			{
				sv.ScrollToVerticalOffset(sv.VerticalOffset - offset); //Scroll up.
			}
			else if (verticalPos > li.ActualHeight - tolerance) //Bottom of visible list?
			{
				sv.ScrollToVerticalOffset(sv.VerticalOffset + offset); //Scroll down.    
			}
		}

		#endregion Drag

		#region Drop

		private void TaskList_Drop(DragEventArgs e)
		{
			LoggerService.Inforamtion(this, "Object is dropped");

			try
			{


				if (e.Data.GetDataPresent("TaskTypeToList"))
				{
					TaskBase TaskBase = e.Data.GetData("TaskTypeToList") as TaskBase;
					AddTask(TaskBase, e);
				}
				else if (e.Data.GetDataPresent("ChangeTaskPlace"))
				{
					MoveNode(e);
				}

				IsChanged = true;

			}
			catch (Exception ex)
			{
				LoggerService.Error(this, "Failed to drop", "Design Error", ex);
			}

		}

		private void DropOfParameter(DragEventArgs e)
		{

		}


		private void GetDroppedAndDroppedOn(
			DragEventArgs e,
			out TaskBase dropped,
			out TaskBase droppedOn)
		{
			dropped = null;
			droppedOn = null;

			IList selectedItems = e.Data.GetData("ChangeTaskPlace") as IList;
			if (selectedItems != null && selectedItems.Count != 0)
			{
				IEnumerator enumerator = selectedItems.GetEnumerator();
				enumerator.MoveNext();
				if (enumerator.Current == null)
					return;

				dropped = enumerator.Current as TaskBase;
			}
			else
			{
				dropped = e.Data.GetData("ChangeTaskPlace") as TaskBase;
			}


			ListViewItem listViewItem =
				FindAncestorService.FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
			if (listViewItem == null)
				return;

			droppedOn = listViewItem.DataContext as TaskBase;
		}

		private void TaskList_DragEnter(DragEventArgs e)
		{
			if (!e.Data.GetDataPresent("TaskTypeToList"))
			{
				e.Effects = DragDropEffects.None;
			}
		}

		#endregion Drop

		#region Up/Down

		private void MoveTaskUp()
		{
			try
			{
				LoggerService.Inforamtion(this, "Moving task UP");

				if (_selectedItems == null || _selectedItems.Count == 0)
					return;

				List<TaskBase> list = new List<TaskBase>();
				foreach (TaskBase task in _selectedItems)
					list.Add(task);

				foreach (TaskBase task in list)
				{
					int index = TasksList.TasksList.IndexOf(task);
					if (index <= 0)
						return;

					TasksList.TasksList.Remove(task);
					TasksList.TasksList.Insert(index - 1, task);
				}

				IsChanged = true;
			}
			catch (Exception ex)
			{
				LoggerService.Error(this, "Failed to move task up", "Up/Down Error", ex);
			}
		}

		private void MoveTaskDown()
		{
			LoggerService.Inforamtion(this, "Moving task DOWN");

			try
			{

				if (_selectedItems == null || _selectedItems.Count == 0)
					return;

				List<TaskBase> list = new List<TaskBase>();
				foreach (TaskBase task in _selectedItems)
					list.Add(task);

				list.Reverse();

				foreach (TaskBase task in list)
				{
					int index = TasksList.TasksList.IndexOf(task);
					if (index < 0 || index >= (TasksList.TasksList.Count - 1))
						return;

					TasksList.TasksList.Remove(task);
					TasksList.TasksList.Insert(index + 1, task);
				}

				IsChanged = true;
			}
			catch (Exception ex)
			{
				LoggerService.Error(this, "Failed to move task down", "Up/Down Error", ex);
			}
		}

		private void MoveNode(DragEventArgs e)
		{
			TaskBase dropedOnTask = null;
			ListViewItem listViewItem =
						FindAncestorService.FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
			if (listViewItem != null)
			{
				dropedOnTask = listViewItem.DataContext as TaskBase;
			}

			if (dropedOnTask == null)
			{
				return;
			}


			if (_selectedItems == null || _selectedItems.Count == 0)
				return;

			List<TaskBase> list = new List<TaskBase>();
			foreach (var item in _selectedItems)
				list.Add(item as TaskBase);

			int indexDroppedOn = TasksList.TasksList.IndexOf(dropedOnTask);

			foreach (TaskBase item in list)
			{
				int indexDropped = TasksList.TasksList.IndexOf(item);

				TasksList.TasksList.RemoveAt(indexDropped);

				if (indexDroppedOn > -1)
					TasksList.TasksList.Insert(indexDroppedOn, item);
				else if (indexDroppedOn >= (TasksList.TasksList.Count - 1))
					TasksList.TasksList.Add(item);
			}

			return;
		}

		#endregion Up/Down

		#region Script file handling

		//public void New()
		//{
		//	ScriptNameView scriptNameView = new ScriptNameView();

		//	scriptNameView.Title = "Task Name";
		//	scriptNameView.SubTitle = "New Task name";
		//	scriptNameView.ButtonTitle = "Create";
		//	scriptNameView.ScriptName = null;

		//	bool? result = scriptNameView.ShowDialog();
		//	if (result != true)
		//	{
		//		return;
		//	}
		//}


		public void Load()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Tasks List files (*.tsk)|*.tsk;";

			//string initDir = "";
			//if (!string.IsNullOrEmpty(_scriptUserData.LastDesignScriptPath))
			//	initDir = _scriptUserData.LastDesignScriptPath;
			//if (Directory.Exists(initDir) == false)
			//	initDir = "";
			//openFileDialog.InitialDirectory = initDir;
			bool? result = openFileDialog.ShowDialog();

			if (result != true)
				return;

			TasksList.TasksListPath = openFileDialog.FileName;

			string jsonString = File.ReadAllText(openFileDialog.FileName);
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.Formatting = Formatting.Indented;
			settings.TypeNameHandling = TypeNameHandling.All;
			TasksList = JsonConvert.DeserializeObject(jsonString, settings) as
				TasksListData;
		}


		public void Save()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			//saveFileDialog.FileName = CurrentScript.Name;
			saveFileDialog.Filter = "Tasks List files (*.tsk)|*.tsk;";
			bool? result = saveFileDialog.ShowDialog();
			if (result != true)
				return;

			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.Formatting = Formatting.Indented;
			settings.TypeNameHandling = TypeNameHandling.All;
			var sz = JsonConvert.SerializeObject(TasksList, settings);
			File.WriteAllText(saveFileDialog.FileName, sz);

			TasksList.TasksListPath = saveFileDialog.FileName;
		}



		#endregion Script file handling


		public void AddTask(
		TaskBase source_TaskBase,
		DragEventArgs e)
		{
			TaskBase new_TaskBase = source_TaskBase.Clone() as TaskBase;

			AddTask_do(
				new_TaskBase,
				source_TaskBase.Name,
				e);
		}

		private void AddTask_do(
			TaskBase new_TaskBase,
			string name,
			DragEventArgs e)
		{
			TasksList.TasksList.Add(new_TaskBase);
		}


		private int GetNewTaskPosition(
			DragEventArgs e,
			out TaskBase dropedOnTask)
		{
			dropedOnTask = null;
			if (e == null)
				return -1;

			ListViewItem listViewItem =
						FindAncestorService.FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
			if (listViewItem != null)
			{
				dropedOnTask = listViewItem.DataContext as TaskBase;
			}

			int index = TasksList.TasksList.IndexOf(dropedOnTask);
			//if (index == -1 || index == (TasksList.Count - 1))
			//	return -1;

			return index;
		}



		private void TaskList_SelectionChanged(SelectionChangedEventArgs e)
		{
			if (!(e.OriginalSource is ListView listView))
				return;

			_selectedItems = listView.SelectedItems;

			_selectedTask = listView.SelectedItem as TaskBase;
			if (_selectedTask == null)
				LoggerService.Inforamtion(this, "Task selected: None");
			else
				LoggerService.Inforamtion(this, "Task selected: " + _selectedTask.Name);
		}





		private void Delete()
		{
			if (_selectedItems == null)
				return;

			List<TaskBase> list = new List<TaskBase>();
			foreach (TaskBase item in _selectedItems)
				list.Add(item);

			foreach (TaskBase item in list)
			{
				int index = TasksList.TasksList.IndexOf(item);
				LoggerService.Inforamtion(this, "Task removed: " + item.Description);
				TasksList.TasksList.Remove(item);

			}

			IsChanged = true;
		}


		private void CopyScriptTask()
		{
			Copy();
			Past();
		}


		private void TaskPropertyChangedHandler(TaskBase sender, string propertyName)
		{
			if (propertyName == "IsExpanded")
			{
				return;
			}

			IsChanged = true;
		}

		private void TaskList_PreviewKeyUp(KeyEventArgs e)
		{
			if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
			{
				//Save(CurrentScript is TestData);
			}
		}

		public bool SaveIfNeeded()
		{
			if (IsChanged)
			{

			}

			return false;
		}


		private void Copy()
		{
			List<(int, TaskBase)> list = new List<(int, TaskBase)>();
			foreach (TaskBase item in _selectedItems)
			{
				int index = TasksList.TasksList.IndexOf(item);
				list.Add((index, item));
			}

			list.Sort((a, b) => a.Item1.CompareTo(b.Item1));
			List<TaskBase> list1 = list.Select((a) => a.Item2).ToList();

			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.Formatting = Formatting.Indented;
			settings.TypeNameHandling = TypeNameHandling.All;
			string copyString = JsonConvert.SerializeObject(list1, settings);


			string format = "MyTask";
			Clipboard.Clear();
			Clipboard.SetData(format, copyString);
		}

		private void Past()
		{
			if (Clipboard.ContainsData("MyTask") == false)
				return;

			string copyString = (string)Clipboard.GetData("MyTask");
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.Formatting = Formatting.Indented;
			settings.TypeNameHandling = TypeNameHandling.All;
			List<TaskBase> list =
				JsonConvert.DeserializeObject(copyString, settings) as List<TaskBase>;

			foreach (TaskBase item in list)
			{

				AddTask_do(
					item as TaskBase,
					item.GetType().Name,
					null);

				//	TasksList[TasksList.Count - 1].PassNext = TasksList[TasksList.Count - 2];
			}


			IsChanged = true;
		}


		private void TextBox_PreviewDragOver(DragEventArgs e)
		{
			e.Handled = true;
		}



		#endregion Methods

		#region Commands
		public RelayCommand LoadedCommand { get; private set; }

		#region Drag


		private RelayCommand<MouseEventArgs> _TaskList_MouseMoveCommand;
		public RelayCommand<MouseEventArgs> TaskList_MouseMoveCommand
		{
			get
			{
				return _TaskList_MouseMoveCommand ?? (_TaskList_MouseMoveCommand =
					new RelayCommand<MouseEventArgs>(TaskList_MouseMove));
			}
		}

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


		private RelayCommand<DragEventArgs> _TaskList_DragOverCommand;
		public RelayCommand<DragEventArgs> TaskList_DragOverCommand
		{
			get
			{
				return _TaskList_DragOverCommand ?? (_TaskList_DragOverCommand =
					new RelayCommand<DragEventArgs>(TaskList_DragOver));
			}
		}

		#endregion Drag

		#region Drop

		private RelayCommand<DragEventArgs> _TaskList_DropCommand;
		public RelayCommand<DragEventArgs> TaskList_DropCommand
		{
			get
			{
				return _TaskList_DropCommand ?? (_TaskList_DropCommand =
					new RelayCommand<DragEventArgs>(TaskList_Drop));
			}
		}

		private RelayCommand<DragEventArgs> _TaskList_DragEnterCommand;
		public RelayCommand<DragEventArgs> TaskList_DragEnterCommand
		{
			get
			{
				return _TaskList_DragEnterCommand ?? (_TaskList_DragEnterCommand =
					new RelayCommand<DragEventArgs>(TaskList_DragEnter));
			}
		}

		#endregion Drop

		public RelayCommand MoveTaskUpCommand { get; private set; }
		public RelayCommand MoveTaskDownCommand { get; private set; }
		public RelayCommand DeleteCommand { get; private set; }
		public RelayCommand CopyScriptTaskCommand { get; private set; }
		public RelayCommand ExportScriptToPDFCommand { get; private set; }

		public RelayCommand ScriptExpandAllCommand { get; private set; }
		public RelayCommand ScriptCollapseAllCommand { get; private set; }

		public RelayCommand<TaskBase> DeleteNextPassCommand { get; private set; }
		public RelayCommand<TaskBase> DeleteNextFailCommand { get; private set; }



		public RelayCommand CopyCommand { get; private set; }
		public RelayCommand PastCommand { get; private set; }

		//public RelayCommand NewCommand { get; private set; }
		public RelayCommand LoadCommand { get; private set; }
		public RelayCommand SaveCommand { get; private set; }




		private RelayCommand<SelectionChangedEventArgs> _TaskList_SelectionChangedCommand;
		public RelayCommand<SelectionChangedEventArgs> TaskList_SelectionChangedCommand
		{
			get
			{
				return _TaskList_SelectionChangedCommand ?? (_TaskList_SelectionChangedCommand =
					new RelayCommand<SelectionChangedEventArgs>(TaskList_SelectionChanged));
			}
		}

		private RelayCommand<KeyEventArgs> _TaskList_PreviewKeyUpCommand;
		public RelayCommand<KeyEventArgs> TaskList_PreviewKeyUpCommand
		{
			get
			{
				return _TaskList_PreviewKeyUpCommand ?? (_TaskList_PreviewKeyUpCommand =
					new RelayCommand<KeyEventArgs>(TaskList_PreviewKeyUp));
			}
		}



		private RelayCommand<DragEventArgs> _TextBox_PreviewDragOverCommand;
		public RelayCommand<DragEventArgs> TextBox_PreviewDragOverCommand
		{
			get
			{
				return _TextBox_PreviewDragOverCommand ?? (_TextBox_PreviewDragOverCommand =
					new RelayCommand<DragEventArgs>(TextBox_PreviewDragOver));
			}
		}

		#endregion Commands

		#region Events

		//public event EventHandler<bool> ScriptIsChangedEvent;
		public event EventHandler ScriptIsSavedEvent;
		public event EventHandler ScriptReloadedEvent;

		#endregion Events
	}
}
