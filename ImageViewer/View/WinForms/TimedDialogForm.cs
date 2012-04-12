using System;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    public partial class TimedDialogForm : DotNetMagicForm
    {
        private readonly double _minOpacity;
        private readonly double _maxOpacity;

        private TimeSpan _lingerTime;
        private Timer _timer;
        private int _startTicks;

        private bool _stay;

        public TimedDialogForm(Control control, string title, TimeSpan lingerTime)
        {
            _lingerTime = lingerTime;
            SuspendLayout();
            
            InitializeComponent();
            _hostPanel.Controls.Add(control);

            /// TODO (CR Apr 2012): This exists in the core framework dialog, too, and possibly elsewhere as well.
            Text = string.IsNullOrEmpty(title) 
                            ? Desktop.Application.Name 
                            : string.Format("{0} - {1}", Desktop.Application.Name, title);

            _maxOpacity = 1.0;
            _minOpacity = 0.3;
            ResumeLayout();

            StartFadeTimer();
        }

        public event EventHandler CloseRequested;

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

                const int bufferX = 2;
                const int bufferY = 2;

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

        private bool IsMouseOver()
        {
            var cursorPosition = Cursor.Position;
            var bounds = new Rectangle(Location, Size);
            return bounds.Contains(cursorPosition);
        }

        private void Stay()
        {
            if (_stay)
                return;

            _stay = true;
            Opacity = 1.0;
            //Once the user puts their mouse over, after that we always fade out in one second if the mouse is outside.
            _lingerTime = TimeSpan.FromSeconds(1);
            Invalidate();
        }

        private void StartFadeTimer()
        {
            _startTicks = Environment.TickCount;
            _timer = new Timer { Interval = 100 };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (IsMouseOver())
            {
                Stay();
                return;
            }
            else if (_stay)
            {
                _stay = false;
                _startTicks = Environment.TickCount;
            }

            var elapsed = Environment.TickCount - _startTicks;
            var remaining = _lingerTime.TotalMilliseconds - elapsed;
            if (remaining <= 0)
            {
                DisposeTimer();
                Close();
                return;
            }

            var ratio = remaining / _lingerTime.TotalMilliseconds;
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
    }
}
