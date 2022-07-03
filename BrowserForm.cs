// Copyright (C) Microsoft Corporation. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.Web.WebView2.Core;
using System.IO;
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
			var userDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "kioskbrowser");
			var options = new Microsoft.Web.WebView2.Core.CoreWebView2EnvironmentOptions
			{
				AdditionalBrowserArguments = "--user-agent=\"kioskbrowser\"",
				AllowSingleSignOnUsingOSPrimaryAccount = true
			};
			var webView2Environment = Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder, options).Result;
			this.webView2Control.EnsureCoreWebView2Async(webView2Environment);

			var args = "";
			if (Environment.GetCommandLineArgs().Length > 1)
			{
				args = Regex.Replace(Environment.GetCommandLineArgs()[1], @"kioskbrowser:\b", "", RegexOptions.IgnoreCase);
				this.webView2Control.Source = new System.Uri($"{args}", System.UriKind.Absolute);

			}
			else
			{
				this.webView2Control.Source = new System.Uri("https://www.bing.com/", System.UriKind.Absolute);
			}
		}


		#region Event Handlers


		private void WebView2Control_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
		{
		//	txtUrl.Text = webView2Control.Source.AbsoluteUri;
		}

		private  void OnChanged(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType != WatcherChangeTypes.Changed)
			{
				return;
			}
			MessageBox.Show("file changed");
			//this.txtUrl.Text = "https://www.google.com/";
			webView2Control.Source = new Uri("https://www.google.com/");
		}
		void AttachControlEventHandlers(Microsoft.Web.WebView2.WinForms.WebView2 control)
		{

			var userDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "kioskbrowser");

			var watcher = new FileSystemWatcher($"{userDataFolder}");

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
	}
}
