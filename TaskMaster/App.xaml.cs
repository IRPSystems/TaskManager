﻿using Controls.Views;
using ControlzEx.Theming;
using Services.Services;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using TaskMaster.Views;

namespace TaskMaster
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
				 "MzM3MDg2M0AzMjM0MmUzMDJlMzBCT2dsKzBPUW9HbXFrM1J3aWxQR2k5UDVOZXNDdE4zdGJCSjI5N2lpWGlJPQ==");

			this.DispatcherUnhandledException += App_DispatcherUnhandledException;


		}

		protected override void OnStartup(StartupEventArgs e)
		{
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			base.OnStartup(e);

			SplashView splash = new SplashView();
			splash.AppName = "Release Tasks Master";
			splash.Show();

			// Right now I'm showing main window right after splash screen but I will eventually wait until splash screen closes.
			MainWindow = new TaskMasterMainWindow();
			MainWindow.Show();
			splash.Close();


		}

		public static void ChangeDarkLight(bool isLightTheme)
		{
			if (Current == null)
				return;

			if (isLightTheme)
				ThemeManager.Current.ChangeTheme(Current, "Light.Cobalt");
			else
				ThemeManager.Current.ChangeTheme(Current, "Dark.Cobalt");
		}



		private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			LoggerService.Error(this, "Un-handled exception caught", "Error", e.Exception);
			e.Handled = true;
		}
	}

}
