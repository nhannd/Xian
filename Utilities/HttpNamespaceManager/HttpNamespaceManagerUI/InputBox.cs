#region License (non-CC)

// Copyright (c) 2007, Paul Wheeler
//
// This work is licensed under a Creative Commons Attribution 3.0 Unported License.
// For the complete license, see http://creativecommons.org/licenses/by/3.0/
// Or, you may send a letter to: 
//    Creative Commons
//    171 Second Street, Suite 300
//    San Francisco, California, 94105, USA.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HttpNamespaceManager.UI
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public InputBox(string title, string prompt)
        {
            InitializeComponent();
            this.Text = title;
            this.labelPrompt.Text = prompt;
            this.Size = new Size(Math.Max(this.labelPrompt.Width + 31, 290), this.labelPrompt.Height + 103);
        }

        public static DialogResult Show(string title, string prompt, out string result)
        {
            InputBox input = new InputBox(title, prompt);
            DialogResult retval = input.ShowDialog();
            result = input.textInput.Text;
            return retval;
        }
    }
}