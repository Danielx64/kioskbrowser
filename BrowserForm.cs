// Copyright (C) Microsoft Corporation. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.Web.WebView2.Core;

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
            txtUrl.Text = webView2Control.Source.AbsoluteUri;
        }

        void AttachControlEventHandlers(Microsoft.Web.WebView2.WinForms.WebView2 control)
        {
            control.SourceChanged += WebView2Control_SourceChanged;
        }

        #endregion

        #region UI event handlers


        private void BtnGo_Click(object sender, EventArgs e)
        {
            var rawUrl = txtUrl.Text;
            Uri uri = null;

            if (Uri.IsWellFormedUriString(rawUrl, UriKind.Absolute))
            {
                uri = new Uri(rawUrl);
            }
            else if (!rawUrl.Contains(" ") && rawUrl.Contains("."))
            {
                // An invalid URI contains a dot and no spaces, try tacking http:// on the front.
                uri = new Uri("http://" + rawUrl);
            }
            else
            {
                // Otherwise treat it as a web search.
                uri = new Uri("https://bing.com/search?q=" +
                    String.Join("+", Uri.EscapeDataString(rawUrl).Split(new string[] { "%20" }, StringSplitOptions.RemoveEmptyEntries)));
            }

            webView2Control.Source = uri;
        }

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
