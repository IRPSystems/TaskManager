using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskMaster.Models;
using TaskMaster.Models.Tasks;

namespace TaskMaster.Converters
{
	public class RemoveBreakLineFromNameConverter : IValueConverter
	{

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(!(value is TaskBase))
				return Brushes.Black;

			if(value is BuildVersionTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000");
			if (value is CopyFilesTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#FF6A00");
			if (value is CreateReleaseNotesTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD800");
			if (value is GitCommandsTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#B6FF00");
			if (value is LinkToJiraTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#4CFF00");
			if (value is LogicConstrainsTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#00FF21");
			if (value is MailToOutlookTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#00FF90");
			if (value is OpenUrlsTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#00FFFF");
			if (value is RenameFilesTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#0094FF");
			if (value is UploadToGithubTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#0026FF");
			if (value is UseExeFilesTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#4800FF");
			if (value is ZipFilesTask)
				return (SolidColorBrush)new BrushConverter().ConvertFrom("#B200FF");


			return Brushes.Black;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.Empty;
		}
	}
}
