// Copyright (C) Microsoft Corporation. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using Microsoft.Web.WebView2.Core;

[assembly: AssemblyTitle("Your app name here")]
//[assembly: AssemblyProduct("1.0.0.0")]
[assembly: AssemblyCompany("Your company here")]
[assembly: AssemblyCopyright("Copyright ©2022")]

namespace WebView2WindowsFormsBrowser
{
	public static class Globals
	{
		public static readonly String APP_ID = "your app id"; // Unmodifiable
		public static readonly String APP_FOLDER_NAME = "your app folder name"; // Unmodifiable
		public static readonly String APP_NAME = "your app name"; // Unmodifiable
		public static readonly String TENANT_ID = "your teant id"; // Unmodifiable
		public static readonly String APP_USERAGENT = "Your useragent here";
		public static readonly String APP_REQUEST_LANG = "en-AU";
		public static readonly String URI_SCHEMA = "kioskbrowser";
		public static readonly String BASE_URL = "https://apps.powerapps.com/play/" + APP_ID + "?tenantId=" + TENANT_ID + "&source=iframe&hidenavbar=true&"; // Unmodifiable
		public static readonly String USER_DATA_FOLDER = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_FOLDER_NAME);
	}
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static Mutex mutex = new Mutex(true, $"{Globals.APP_ID}");

		[STAThread]

		static void Main()
		{
			if (mutex.WaitOne(TimeSpan.Zero, true))
			{
				try
				{
					var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
					// Do something with `version` if needed.
					Application.SetHighDpiMode(HighDpiMode.SystemAware);
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new BrowserForm());
				}
				catch (WebView2RuntimeNotFoundException exception)
				{
					// Handle the runtime not being installed.
					// `exception.Message` is very nicely specific: It (currently at least) says "Couldn't find a compatible Webview2 Runtime installation to host WebViews."
					MessageBox.Show(exception.Message);
					Environment.Exit(0);
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
			else
			{
				string filePath = @Globals.USER_DATA_FOLDER + @"\temp.txt";
				var outString = BrowserForm.RemoveSpecialChars(Environment.GetCommandLineArgs()[1]);
				using (StreamWriter outputFile = new StreamWriter(filePath))
				{
					outputFile.WriteLine(outString);
				}
				Environment.Exit(0);
			}
		}
	}
}
