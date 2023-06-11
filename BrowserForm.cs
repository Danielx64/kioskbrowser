// Copyright (C) Microsoft Corporation. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Microsoft.Web.WebView2.WinForms;
using System.Threading;
using System.Collections.Generic;

namespace WebView2WindowsFormsBrowser
{
	public partial class BrowserForm : Form
	{
		public BrowserForm()
		{
					InitializeComponent();
			InitializeBrowser();
					AttachControlEventHandlers();
					HandleResize();
		}
		private void InitializeBrowser()
		{
			var options = new CoreWebView2EnvironmentOptions
			{
				AllowSingleSignOnUsingOSPrimaryAccount = true,
				Language = $"{Globals.APP_REQUEST_LANG}",
			};

			if (!Directory.Exists(Globals.USER_DATA_FOLDER))
			{
				Directory.CreateDirectory(Globals.USER_DATA_FOLDER);
			}
			var webView2Environment = CoreWebView2Environment.CreateAsync(null, Globals.USER_DATA_FOLDER, options).Result;

			_ = this.webView2Control.EnsureCoreWebView2Async(webView2Environment);

			if (Environment.GetCommandLineArgs().Length > 1)
			{
				var outString = BrowserForm.RemoveSpecialChars(Environment.GetCommandLineArgs()[1]);
				//Add code to check for gpu pram
				if (outString.StartsWith("gpu"))
				{
					this.webView2Control.Source = new Uri($"edge://gpu", UriKind.Absolute);
				}
				else
				{
					this.webView2Control.Source = new Uri($"{Globals.BASE_URL}{outString}", UriKind.Absolute);
				}
			}
			else
			{
				this.webView2Control.Source = new Uri($"{Globals.BASE_URL}", UriKind.Absolute);
			}
		}

		#region Event Handlers

		private  void OnChanged(object sender, FileSystemEventArgs e)
		{
			var milliseconds = 100;
			Thread.Sleep(milliseconds);
			if (e.ChangeType != WatcherChangeTypes.Changed)
			{
				return;
			}
			string filePath = @Globals.USER_DATA_FOLDER + @"\temp.txt";
			using (StreamReader inputFile = new StreamReader(filePath))
			{
				if (RemoveSpecialChars(inputFile.ReadToEnd()).StartsWith("gpu"))
				{
					webView2Control.Source = new Uri($"edge://gpu", UriKind.Absolute);
				}
				else
				{
					webView2Control.Source = new Uri($"{Globals.BASE_URL}" + RemoveSpecialChars(inputFile.ReadToEnd()));
				}
			}
			this.Focus();
			this.Activate();
		}
		void AttachControlEventHandlers()
		{
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher($"{Globals.USER_DATA_FOLDER}");
			var watcher = fileSystemWatcher;
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
			webView2Control.Size = this.ClientSize - new Size(webView2Control.Location);
		}
		private void Form1_Closing(object sender, FormClosingEventArgs e)
		{
			webView2Control.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache", "{}");
		}

		private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
		{
			e.NewWindow = webView2Control.CoreWebView2;
		}

		private void WebView2Control_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
		{
			this.webView2Control.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
			this.webView2Control.CoreWebView2.Settings.AreDevToolsEnabled = false;
			this.webView2Control.CoreWebView2.Settings.IsStatusBarEnabled = false;
			this.webView2Control.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
			this.webView2Control.CoreWebView2.Settings.AreHostObjectsAllowed = false;
			this.webView2Control.CoreWebView2.Settings.IsWebMessageEnabled = false;
			this.webView2Control.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
			this.webView2Control.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
			this.webView2Control.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
			this.webView2Control.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
			this.webView2Control.CoreWebView2.ContextMenuRequested += menurequested;
		}
		public static string RemoveSpecialChars(string str)
		{

			str = str.Replace($"{Globals.URI_SCHEMA}", "");
			// Create  a string array and add the special characters you want to remove
			string[] chars = new string[] { "~", "`", "!", "@", "#", "$", "%", "^", "*", "(", ")", "_", "+", "}", "{", "]", "[", "|", "\"", ":", "'", ":", ">", "<", "/", ".", ",", "\\" };

			//Iterate the number of times based on the String array length.
			for (int i = 0; i < chars.Length; i++)
			{
				if (str.Contains(chars[i]))
				{
					str = str.Replace(chars[i], "");
				}
			}

			return str;
		}

		private void menurequested(object sender, CoreWebView2ContextMenuRequestedEventArgs args)
		{

			IList<CoreWebView2ContextMenuItem> menuList = args.MenuItems;
			CoreWebView2ContextMenuTargetKind context = args.ContextMenuTarget.Kind;
			if (context == CoreWebView2ContextMenuTargetKind.Audio)
			{
				for (int index = menuList.Count - 1; index >= 0; index--)
				{
					menuList.RemoveAt(index);
				}
			}
			if (context == CoreWebView2ContextMenuTargetKind.Image)
			{
				for (int index = menuList.Count - 1; index >= 0; index--)
				{
					menuList.RemoveAt(index);
				}
			}
			if (context == CoreWebView2ContextMenuTargetKind.Page)
			{
				for (int index = menuList.Count - 1; index >= 0; index--)
				{
					if (menuList[index].Name != "reload") { menuList.RemoveAt(index); }
				}
			}
			if (context == CoreWebView2ContextMenuTargetKind.SelectedText)
			{
				for (int index = menuList.Count - 1; index >= 0; index--)
				{
					if (menuList[index].Name != "copy" && menuList[index].Name != "paste" && menuList[index].Name != "cut") { menuList.RemoveAt(index); }
				}
			}
			if (context == CoreWebView2ContextMenuTargetKind.Video)
			{
				for (int index = menuList.Count - 1; index >= 0; index--)
				{
					menuList.RemoveAt(index);
				}
			}

		}
	}
}
