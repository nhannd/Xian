using System;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class TimedDialogForm : DotNetMagicForm
    {
        private readonly double _minOpacity;
        private readonly double _maxOpacity;

        private TimeSpan _lingerTime;
        private int _startTicks;

        private bool _stay;
    	private int _slot;
		private Action _linkHandler;
    	private bool _autoDismiss;

        public TimedDialogForm(DesktopForm owner, string title)
        {
             SuspendLayout();
            
            InitializeComponent();

        	this.Text = title;
        	this.Owner = owner;

            _maxOpacity = 1.0;
            _minOpacity = 0.3;
            ResumeLayout();

        }

    	public bool AutoDismiss
    	{
			get { return _autoDismiss; }
			set { _autoDismiss = value; }
    	}

    	public string Message
    	{
			get { return _message.Text; }
			set { _message.Text = value; }
    	}

		public string LinkText
		{
			get { return _contextualLink.Text; }
			set { _contextualLink.Text = value; }
		}

		public Action LinkHandler
		{
			get { return _linkHandler; }
			set { _linkHandler = value; }
		}

		public void Popup(int slot)
		{
			// reset all the parameters
			_slot = slot;
			this.Opacity = 1.0;
			_lingerTime = TimeSpan.FromSeconds(3);
			_startTicks = Environment.TickCount;

			SetLocation();

			if (!this.Visible)
				Show();

			_timer.Start();
		}

		public void Show(DesktopForm owner, int slot)
		{
			Show(owner);
		}

        protected override void OnLoad(EventArgs e)
        {
			base.OnLoad(e);
        }

		private void _contextualLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (_linkHandler != null)
			{
				_linkHandler();
			}
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

				var location = new Point(ownerRight - width, ownerBottom - height * (_slot + 1));
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

        private void OnTimerTick(object sender, EventArgs e)
        {
			if (!_autoDismiss)
				return;

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
            	Dismiss();
            	return;
            }

        	var ratio = remaining / _lingerTime.TotalMilliseconds;
            var addOpacity = (_maxOpacity - _minOpacity) * ratio;
            Opacity = _minOpacity + addOpacity;

            if (Opacity <= _minOpacity)
            {
				Dismiss();
			}
            else
            {
                Invalidate();
            }
        }

    	private void Dismiss()
    	{
			_timer.Stop();
			Hide();
    	}

		private void TimedDialogForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(e.CloseReason == System.Windows.Forms.CloseReason.UserClosing)
			{
				e.Cancel = true;
				Hide();
			}
		}

    }
}
