// Copyright (C) Microsoft Corporation. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
namespace WebView2WindowsFormsBrowser
{
	public static class Globals
	{
		public static readonly String APP_ID = "your app id"; // Unmodifiable
		public static readonly String APP_FOLDER_NAME = "your app folder name"; // Unmodifiable
		public static readonly String APP_NAME = "your app name"; // Unmodifiable
		public static readonly String TENANT_ID = "your teant id"; // Unmodifiable
		public static readonly String BASE_URL = "https://apps.powerapps.com/play/" + APP_ID + "tenantId=" + TENANT_ID + "&"; // Unmodifiable
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
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new BrowserForm());
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
			else
			{
				var args = "";

				string filePath = @Globals.USER_DATA_FOLDER + @"\temp.txt";
				args = Regex.Replace(Environment.GetCommandLineArgs()[1], @"kioskbrowser:\b", "", RegexOptions.IgnoreCase);

				using (StreamWriter outputFile = new StreamWriter(filePath))
				{
					outputFile.WriteLine(args);
				}

				Environment.Exit(0);
			}
		}
	}
}
