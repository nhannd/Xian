using System;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    public partial class TimedDialogForm : DotNetMagicForm
    {
        private readonly TimeSpan _lingerTimer;
        private Timer _timer;
        private readonly int _startTicks;
        private readonly double _minOpacity;
        private readonly double _maxOpacity;
        private bool _stay;

        public TimedDialogForm(Control control, string title, TimeSpan lingerTimer)
        {
            _lingerTimer = lingerTimer;
            SuspendLayout();
            
            InitializeComponent();
            _hostPanel.Controls.Add(control);

            /// TODO (CR Apr 2012): This exists in the core framework dialog, too, and possibly elsewhere as well.
            Text = string.IsNullOrEmpty(title) 
                            ? Desktop.Application.Name 
                            : string.Format("{0} - {1}", Desktop.Application.Name, title);

            _minOpacity = 0.3;
            Opacity = _maxOpacity = 1.0;

            control.MouseEnter += OnControlMouseEnter;
            control.MouseLeave += OnControlMouseLeave;
            ResumeLayout();

            _startTicks = Environment.TickCount;
            _timer = new Timer { Interval = 100 };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        public event EventHandler CloseRequested;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Stay();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_stay)
                Close();
        }

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
            if (_stay)
                return;

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

        private void OnControlMouseEnter(object sender, EventArgs e)
        {
            Stay();
        }

        private void OnControlMouseLeave(object sender, EventArgs e)
        {
            Close();
        }

        private void Stay()
        {
            _stay = true;
            Opacity = 1.0;
            Invalidate();
            DisposeTimer();
        }
    }
}
