#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// This class is experimental, does not entirely work right now.
    /// </summary>
    public partial class SmartTimeField : UserControl
    {
        private List<TimeSpan> _choices = new List<TimeSpan>();

        private TimeSpan _minimum = TimeSpan.Zero + TimeSpan.FromHours(7);      // 7am
        private TimeSpan _maximum = TimeSpan.Zero + TimeSpan.FromHours(7+24);   // 7am tomorrow
        private TimeSpan _interval = TimeSpan.FromMinutes(30);  // 30 mins

        public SmartTimeField()
        {
            InitializeComponent();
            _input.SuggestionProvider = new DefaultSuggestionProvider<TimeSpan>(_choices, FormatTimeSpan);
            _input.Format += new ListControlConvertEventHandler(InputFormatEventHandler);
        }


        public TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                if (!DesignMode)
                {
                    GenerateChoices();
                }
            }
        }

        public TimeSpan Minimum
        {
            get { return _minimum; }
            set
            {
                _minimum = value;
                if (!DesignMode)
                {
                    GenerateChoices();
                }
            }
        }

        public TimeSpan Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                if (!DesignMode)
                {
                    GenerateChoices();
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if(!this.DesignMode)
            {
                GenerateChoices();
            }

            base.OnLoad(e);
        }

        private void GenerateChoices()
        {
            _choices.Clear();
            for (TimeSpan value = _minimum; value < _maximum; value += _interval)
                _choices.Add(value);
        }

        private string FormatTimeSpan(TimeSpan ts)
        {
            DateTime time = DateTime.Today + ts;
            return Format.Time(time);
        }

        private void InputFormatEventHandler(object sender, ListControlConvertEventArgs e)
        {
            e.Value = FormatTimeSpan((TimeSpan)e.ListItem);
        }

    }
}
