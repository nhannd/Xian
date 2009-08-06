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