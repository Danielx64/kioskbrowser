﻿// Copyright (C) Microsoft Corporation. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;


namespace WebView2WindowsFormsBrowser
{
	partial class BrowserForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.webView2Control = new Microsoft.Web.WebView2.WinForms.WebView2();
			((System.ComponentModel.ISupportInitialize)(this.webView2Control)).BeginInit();
			this.SuspendLayout();
			// 
			// webView2Control
			// 
			this.webView2Control.AllowExternalDrop = true;
			this.webView2Control.CreationProperties = null;
			this.webView2Control.DefaultBackgroundColor = System.Drawing.Color.White;
			this.webView2Control.Location = new System.Drawing.Point(0, 0);
			this.webView2Control.Margin = new System.Windows.Forms.Padding(2);
			this.webView2Control.Name = "webView2Control";
			this.webView2Control.Size = new System.Drawing.Size(925, 524);
			this.webView2Control.TabIndex = 7;
			this.webView2Control.ZoomFactor = 1D;
			this.webView2Control.CoreWebView2InitializationCompleted += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs>(this.WebView2Control_CoreWebView2InitializationCompleted);
			// 
			// BrowserForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(922, 519);
			this.Controls.Add(this.webView2Control);
			this.Name = "BrowserForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = Globals.APP_NAME;
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_Closing);
			this.Resize += new System.EventHandler(this.Form_Resize);
			((System.ComponentModel.ISupportInitialize)(this.webView2Control)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private Microsoft.Web.WebView2.WinForms.WebView2 webView2Control;
	}
}
