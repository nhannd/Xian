using System;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class AlertNotificationForm : DotNetMagicForm
    {
		public class DismissedEventArgs : EventArgs
		{
			internal DismissedEventArgs(bool auto)
			{
				this.AutoDismissed = auto;
			}

			public bool AutoDismissed { get; private set; }
		}


        private readonly double _minOpacity;
        private readonly double _maxOpacity;

        private TimeSpan _lingerTime;
        private int _startTicks;

        private bool _stay;
    	private int _slot;

    	private readonly Bitmap _closeButtonBitmap;
    	private bool _mouseOverClose;
    	private bool _mouseDownOnClose;

        public AlertNotificationForm(DesktopForm owner, string title)
        {
            //SuspendLayout();
            
            InitializeComponent();

			this.Text = title;
        	this.Owner = owner;

            _maxOpacity = 1.0;
            _minOpacity = 0.3;
           // ResumeLayout();

        	var resolver = new ResourceResolver(typeof (AlertNotificationForm).Assembly);
			using (var s = resolver.OpenResource("close.bmp"))
			{
				_closeButtonBitmap = new Bitmap(s);
			}
        }

    	public event EventHandler<DismissedEventArgs> Dismissed;
		public event EventHandler OpenLogClicked;

    	public bool AutoDismiss { get; set; }

    	public Image AlertIcon
    	{
			get { return _icon.Image; }
			set { _icon.Image = value; }
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

    	public Action LinkHandler { get; set; }

    	public string OpenLogLinkText
    	{
			get { return _openLogLink.Text; }
			set { _openLogLink.Text = value; }
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

		private void _contextualLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (LinkHandler != null)
			{
				LinkHandler();
			}
		}

		private void _openLogLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			EventsHelper.Fire(OpenLogClicked, this, EventArgs.Empty);
		}

		private void SetLocation()
        {
            var owner = Owner;
            var ownerLocation = owner.Location;
            var ownerWidth = owner.Bounds.Width;
            var ownerHeight = owner.Bounds.Height;

            const int bufferX = 10;
            const int bufferY = 10;

            var ownerRight = ownerLocation.X + ownerWidth;
            var ownerBottom = ownerLocation.Y + ownerHeight;

			var location = new Point(ownerRight - Bounds.Width - bufferX, ownerBottom - Bounds.Height * (_slot + 1) - bufferY);
            if (location.X < ownerLocation.X)
                location.X = ownerLocation.X;
            if (location.Y < ownerLocation.Y)
                location.Y = ownerLocation.Y;

            Location = location;
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
			if (!AutoDismiss)
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
            	Dismiss(true);
            	return;
            }

        	var ratio = remaining / _lingerTime.TotalMilliseconds;
            var addOpacity = (_maxOpacity - _minOpacity) * ratio;
            Opacity = _minOpacity + addOpacity;

            if (Opacity <= _minOpacity)
            {
				Dismiss(true);
			}
            else
            {
                Invalidate();
            }
        }

    	private void Dismiss(bool auto)
    	{
			_timer.Stop();
			Hide();

			EventsHelper.Fire(Dismissed, this, new DismissedEventArgs(auto));
    	}

		private void TimedDialogForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(e.CloseReason == System.Windows.Forms.CloseReason.UserClosing)
			{
				e.Cancel = true;
				Dismiss(false);
			}
		}

		#region Close button event handlers

		private void _closeButton_Click(object sender, EventArgs e)
		{
			Dismiss(false);
		}

		private void _closeButton_Paint(object sender, PaintEventArgs e)
		{
			DrawCloseButton(e.Graphics);
		}

		private void _closeButton_MouseEnter(object sender, EventArgs e)
		{
			_mouseOverClose = true;
			_closeButton.Invalidate();
		}

		private void _closeButton_MouseLeave(object sender, EventArgs e)
		{
			_mouseOverClose = false;
			_closeButton.Invalidate();
		}

		private void _closeButton_MouseDown(object sender, MouseEventArgs e)
		{
			_mouseDownOnClose = true;
			_closeButton.Invalidate();
		}

		private void _closeButton_MouseUp(object sender, MouseEventArgs e)
		{
			_mouseDownOnClose = false;
			_closeButton.Invalidate();
		}

		private void DrawCloseButton(Graphics g)
		{
			var size = new Size(13, 13);
			var rectDest = new Rectangle(new Point(0, 0), size);
			Rectangle rectSrc;

			if (_mouseOverClose)
			{
				rectSrc = _mouseDownOnClose ? new Rectangle(new Point(size.Width * 2, 0), size) : new Rectangle(new Point(size.Width, 0), size);
			}
			else
				rectSrc = new Rectangle(new Point(0, 0), size);

			g.DrawImage(_closeButtonBitmap, rectDest, rectSrc, GraphicsUnit.Pixel);
		}

		#endregion


	}
}
