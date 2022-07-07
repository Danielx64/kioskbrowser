// Copyright (C) Microsoft Corporation. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Microsoft.Web.WebView2.WinForms;

namespace WebView2WindowsFormsBrowser
{

	public partial class BrowserForm : Form
	{

		public BrowserForm()
		{
					InitializeComponent();
					InitializeBrowser();
					AttachControlEventHandlers(this.webView2Control);
					HandleResize();
		}
		private async void InitializeBrowser()
		{
			var options = new Microsoft.Web.WebView2.Core.CoreWebView2EnvironmentOptions
			{
			//	AdditionalBrowserArguments = "--user-agent=\"kioskbrowser\"",
				AllowSingleSignOnUsingOSPrimaryAccount = true,

			};
			var webView2Environment = CoreWebView2Environment.CreateAsync(null, Globals.USER_DATA_FOLDER, options).Result;

			this.webView2Control.EnsureCoreWebView2Async(webView2Environment);

			var args = "";
			if (Environment.GetCommandLineArgs().Length > 1)
			{
				args = Regex.Replace(Environment.GetCommandLineArgs()[1], @"kioskbrowser:\b", "", RegexOptions.IgnoreCase);
				this.webView2Control.Source = new System.Uri($"{Globals.BASE_URL}{args}", System.UriKind.Absolute);
				
			}
			else
			{

				this.webView2Control.Source = new System.Uri($"{Globals.BASE_URL}", System.UriKind.Absolute);
			}
		}

		#region Event Handlers

		private  void OnChanged(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType != WatcherChangeTypes.Changed)
			{
				return;
			}
			string filePath = @Globals.USER_DATA_FOLDER + @"\temp.txt";
			using (StreamReader inputFile = new StreamReader(filePath))
			{
				webView2Control.Source = new Uri($"{Globals.BASE_URL}"+inputFile.ReadToEnd());
			}
		}
		void AttachControlEventHandlers(Microsoft.Web.WebView2.WinForms.WebView2 control)
		{

			var watcher = new FileSystemWatcher($"{Globals.USER_DATA_FOLDER}");

			watcher.NotifyFilter = NotifyFilters.LastWrite;

			watcher.Changed += OnChanged;

			watcher.Filter = "temp.txt";
			watcher.IncludeSubdirectories = false;
			watcher.EnableRaisingEvents = true;
			watcher.SynchronizingObject = this;
		}

		#endregion

		#region UI event handlers



		private void Form_Resize(object sender, EventArgs e)
		{
			HandleResize();
		}


		#endregion

		private void HandleResize()
		{
			// Resize the webview
			webView2Control.Size = this.ClientSize - new System.Drawing.Size(webView2Control.Location);
		}
		private void Form1_Closing(object sender, FormClosingEventArgs e)
		{
			webView2Control.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache", "{}");
			return;
		}

		private void webView2Control_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
		{
			this.webView2Control.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
			this.webView2Control.CoreWebView2.Settings.AreDevToolsEnabled = false;
			this.webView2Control.CoreWebView2.Settings.IsStatusBarEnabled = false;
			this.webView2Control.CoreWebView2.Settings.UserAgent = "Test";
			this.webView2Control.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
		}
	}

}
