﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    public partial class TimedDialogForm : DotNetMagicForm
    {
        private readonly TimeSpan _lingerTimer;
        private Timer _timer;
        private int _startTicks;
        private double _minOpacity;
        private double _maxOpacity;

        public TimedDialogForm(Control control, string title, TimeSpan lingerTimer)
        {
            _lingerTimer = lingerTimer;
            SuspendLayout();
            
            InitializeComponent();
            _hostPanel.Controls.Add(control);

            Text = string.IsNullOrEmpty(title) 
                            ? Desktop.Application.Name 
                            : string.Format("{0} - {1}", Desktop.Application.Name, title);

            _minOpacity = 0.3;
            Opacity = _maxOpacity = 1.0;

            ResumeLayout();

            _startTicks = Environment.TickCount;
            _timer = new Timer { Interval = 100 };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        public event EventHandler CloseRequested;

        private void OnTimerTick(object sender, EventArgs e)
        {
            var elapsed = Environment.TickCount - _startTicks;
            var remaining = _lingerTimer.TotalMilliseconds - elapsed;
            if (remaining <= 0)
            {
                DisposeTimer();
                Close();
                return;
            }

            var ratio = remaining / _lingerTimer.TotalMilliseconds;
            var addOpacity = (_maxOpacity - _minOpacity) * ratio;
            Opacity = _minOpacity + addOpacity;

            if (Opacity <= _minOpacity)
            {
                DisposeTimer();
                Close();
            }
            else
            {
                Invalidate();
            }
        }

        private void DisposeTimer()
        {
            if (_timer == null) return;
            
            _timer.Dispose();
            _timer = null;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            DisposeTimer();
            Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            SetLocation();
            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (CloseRequested != null)
            {
                e.Cancel = true;
                CloseRequested(this, EventArgs.Empty);
            }

            base.OnFormClosing(e);
        }

        private void SetLocation()
        {
            var owner = Owner;
            if (owner == null)
            {
                CenterToScreen();
            }
            else
            {
                var ownerLocation = owner.Location;
                var ownerWidth = owner.Bounds.Width;
                var ownerHeight = owner.Bounds.Height;

                const int bufferX = 20;
                const int bufferY = 20;

                var ownerRight = ownerLocation.X + ownerWidth;
                var ownerBottom = ownerLocation.Y + ownerHeight;

                var width = Bounds.Width + bufferX;
                var height = Bounds.Height + bufferY;

                var location = new Point(ownerRight - width, ownerBottom - height);
                if (location.X < ownerLocation.X)
                    location.X = ownerLocation.X;
                if (location.Y < ownerLocation.Y)
                    location.Y = ownerLocation.Y;

                Location = location;
            }
        }
    }
}
