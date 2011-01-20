#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	internal sealed class ProgressTask
	{
		private Dictionary<string, ProgressTask> _subtasks = null;
		private int _subtaskPoints = 0;
		private int _totalPoints = 100;
		private int _points = 0;

		public ProgressTask()
		{
			_subtasks = new Dictionary<string, ProgressTask>(0);
		}

		private ProgressTask(int totalPoints)
			: this()
		{
			_totalPoints = totalPoints;
		}

		public bool HasSubTasks
		{
			get { return _subtasks.Count > 0; }
		}

		public int TotalPoints
		{
			get { return _totalPoints; }
		}

		public int Points
		{
			get
			{
				if (HasSubTasks)
				{
					return (int) (Progress*_totalPoints);
				}
				else
				{
					return _points;
				}
			}
			set
			{
				if (HasSubTasks)
					return;
				if (value >= 0 && value <= _totalPoints)
					_points = value;
			}
		}

		public float Progress
		{
			get
			{
				if (HasSubTasks)
				{
					float subtaskProgressTotal = 0;
					foreach (ProgressTask subtask in _subtasks.Values)
					{
						subtaskProgressTotal += subtask.Progress*subtask.TotalPoints;
					}
					if (_subtaskPoints == 0)
						return 1f;
					return subtaskProgressTotal/_subtaskPoints;
				}
				else
				{
					if (_totalPoints == 0)
						return 1f;
					return 1f*_points/_totalPoints;
				}
			}
			set
			{
				if (HasSubTasks)
					return;
				if (value >= 0 && value <= 1)
					_points = (int) (_totalPoints*value);
			}
		}

		public float Percent
		{
			get { return this.Progress*100; }
			set { this.Progress = value/100; }
		}

		public int IntPercent
		{
			get { return (int) (this.Progress*100); }
			set { this.Progress = value/100; }
		}

		public ProgressTask AddSubTask(string subtask)
		{
			return this.AddSubTask(subtask, 1);
		}

		public ProgressTask AddSubTask(string subtask, int totalPoints)
		{
			if (_subtasks.ContainsKey(subtask))
				throw new ArgumentException();

			ProgressTask taskitem = new ProgressTask(totalPoints);
			_subtasks.Add(subtask, taskitem);
			_subtaskPoints += totalPoints;
			return taskitem;
		}

		public void Increment()
		{
			if (HasSubTasks)
				return;
			if (_points < _totalPoints)
				_points++;
		}

		public void MarkIncomplete()
		{
			if (HasSubTasks)
			{
				foreach (ProgressTask subtask in _subtasks.Values)
				{
					subtask.MarkIncomplete();
				}
			}
			else
			{
				_points = 0;
			}
		}

		public void MarkComplete()
		{
			if (HasSubTasks)
			{
				foreach (ProgressTask subtask in _subtasks.Values)
				{
					subtask.MarkComplete();
				}
			}
			else
			{
				_points = _totalPoints;
			}
		}

		public ProgressTask this[string subtask]
		{
			get
			{
				if (_subtasks.ContainsKey(subtask))
					return _subtasks[subtask];
				return null;
			}
		}
	}
}