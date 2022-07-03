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
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static Mutex mutex = new Mutex(true, $"{{Globals.APP_ID}}");

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
