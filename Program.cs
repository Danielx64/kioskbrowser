// Copyright (C) Microsoft Corporation. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Windows.Forms;
using System.Threading;
namespace WebView2WindowsFormsBrowser
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
	   static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

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
	MessageBox.Show("only one instance at a time");
	Environment.Exit(0);
}
		}
	}
}
