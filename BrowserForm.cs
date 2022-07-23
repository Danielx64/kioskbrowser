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
using System.Runtime.InteropServices;

namespace WebView2WindowsFormsBrowser
{
	public partial class BrowserForm : Form
	{
		public BrowserForm()
		{
					InitializeComponent();
			UseImmersiveDarkMode(this.Handle, true);
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
			using (StreamReader inputFile = new(filePath))
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
			FileSystemWatcher fileSystemWatcher = new($"{Globals.USER_DATA_FOLDER}");
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
			this.webView2Control.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
			this.webView2Control.CoreWebView2.Settings.AreDevToolsEnabled = false;
			this.webView2Control.CoreWebView2.Settings.IsStatusBarEnabled = false;
			this.webView2Control.CoreWebView2.Settings.UserAgent = $"{Globals.APP_USERAGENT}";
			this.webView2Control.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
			this.webView2Control.CoreWebView2.Settings.AreHostObjectsAllowed = false;
			this.webView2Control.CoreWebView2.Settings.IsWebMessageEnabled = false;
			this.webView2Control.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
			this.webView2Control.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
			this.webView2Control.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
			this.webView2Control.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
		}
		public static string RemoveSpecialChars(string str)
		{
			// Create  a string array and add the special characters you want to remove
			string[] chars = new string[] {"~", "`", "!", "@", "#", "$", "%", "^", "*", "(", ")", "_", "+", "}", "{", "]", "[", "|", "\"", ":", "'", ":", "?", ">", "<", "/", ".", ",","\\"};

			//Iterate the number of times based on the String array length.
			for (int i = 0; i < chars.Length; i++)
			{
				if (str.Contains(chars[i]))
				{
					str = str.Replace(chars[i], "");
				}
			}
			str = str.Replace($"{Globals.URI_SCHEMA}", "");
			return str;
		}

		/*
using System.Runtime.InteropServices;
*/

		[DllImport("dwmapi.dll")]
		private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

		private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
		private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

		private static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
		{
			if (IsWindows10OrGreater(17763))
			{
				var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
				if (IsWindows10OrGreater(18985))
				{
					attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
				}

				int useImmersiveDarkMode = enabled ? 1 : 0;
				return DwmSetWindowAttribute(handle, (int)attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
			}

			return false;
		}

		private static bool IsWindows10OrGreater(int build = -1)
		{
			return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
		}
	}
}
