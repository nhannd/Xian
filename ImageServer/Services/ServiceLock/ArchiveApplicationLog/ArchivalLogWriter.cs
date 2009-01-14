#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using Ionic.Utils.Zip;

namespace ClearCanvas.ImageServer.Services.ServiceLock.ArchiveApplicationLog
{
	/// <summary>
	/// A log file for archiving
	/// </summary>
	class ArchiveLogFile : IDisposable
	{
		private MemoryStream _ms = null;
		private StreamWriter _sw = null;
	
		public ArchiveLogFile(DateTime timestamp, string archivePath, int logSize)
		{
			FirstTimestamp = timestamp;
			Date = timestamp.Date;
			ZipDirectory = Path.Combine(archivePath, Date.ToString("yyyy-MM"));
			ZipFile = Path.Combine(ZipDirectory, String.Format("ImageServerLog_{0}.zip", Date.ToString("yyyy-MM-dd")));
			_ms = new MemoryStream(logSize + 3 * 1024);
			_sw = new StreamWriter(_ms);
		}

		public DateTime FirstTimestamp;
		public DateTime LastTimestamp;
		public string ZipFile;
		public string ZipDirectory;
		public DateTime Date;

		public string LogFileName
		{
			get
			{
				return String.Format("ImageServer_{0}_to_{1}.log", FirstTimestamp.ToString("yyyy-MM-dd_HH-mm-ss"),
					LastTimestamp.ToString("HH-mm-ss"));
			}
		}
	
		public MemoryStream Stream
		{
			get { return _ms; }
		}

		public void Write(ApplicationLog log)
		{
			if (log.Exception != null && log.Exception.Length > 0)
			{
				_sw.WriteLine("{0} {1} [{2}]  {3} - Exception thrown",
				              log.Host,
				              log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				              log.Thread,
				              log.LogLevel);
				_sw.WriteLine(log.Message);
				_sw.WriteLine(log.Exception);
			}
			else
				_sw.WriteLine("{0} {1} [{2}]  {3} - {4}",
				              log.Host,
				              log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				              log.Thread,
				              log.LogLevel,
				              log.Message);
			_sw.Flush();
			LastTimestamp = log.Timestamp;
		}

		public void Dispose()
		{
			if (_sw != null)
			{
				_sw.Close();
				_sw.Dispose();
				_sw = null;
			}
			if (_ms != null)
			{
				_ms.Close();
				_ms.Dispose();
				_ms = null;
			}
		}
	}

	/// <summary>
	/// Simple writer class for archiving log data into zip files.
	/// </summary>
	/// <remarks>
	/// The class assumes that logs are supplied in increasing time order.  It will
	/// write a max of 10MB to a specific log file, then create a new log file with the
	/// name containing a time stamp.
	/// </remarks>
	public class ArchivalLogWriter : IDisposable
	{
		private readonly string _logDirectory;
		private readonly int _logSize = 10*1024*1024;
		private ArchiveLogFile _archiveLog;

		public ArchivalLogWriter(string logDirectory)
		{
			_logDirectory = logDirectory;
		}

		/// <summary>
		/// Write to the log.
		/// </summary>
		/// <param name="log"></param>
		/// <returns>true, if the logs have been flushed.</returns>
		public bool WriteLog(ApplicationLog log)
		{
			DateTime logDate = log.Timestamp.Date;
			if (_archiveLog == null)
			{
				_archiveLog = new ArchiveLogFile(log.Timestamp, _logDirectory, _logSize);
				Platform.Log(LogLevel.Info, "Starting archival of logs for {0}", _archiveLog.FirstTimestamp.ToShortDateString());
			}

			if (logDate.Equals(_archiveLog.Date))
			{
				_archiveLog.Write(log);
				if (_archiveLog.Stream.Length > _logSize)
				{
					FlushLog();
					return true;
				}
				return false;
			}


			// Flush the current log
			FlushLog();
		
			// Simple recursive call to rewrite, since the log has been flushed, will only go 1 deep
			// on the recursion.
			WriteLog(log);

			return true;
		}

		public void FlushLog()
		{
			if (_archiveLog == null) return;

			Platform.Log(LogLevel.Info, "Flushing log for {0}", _archiveLog.FirstTimestamp.ToShortDateString());

			if (!Directory.Exists(_logDirectory))
				Directory.CreateDirectory(_logDirectory);
			if (!Directory.Exists(_archiveLog.ZipDirectory))
				Directory.CreateDirectory(_archiveLog.ZipDirectory);

			using (ZipFile zip = File.Exists(_archiveLog.ZipFile) ? 
					ZipFile.Read(_archiveLog.ZipFile) :
					new ZipFile(_archiveLog.ZipFile))
			{
				ZipEntry e = zip.AddFileStream(_archiveLog.LogFileName, string.Empty, _archiveLog.Stream);
				e.Comment =
					String.Format("Log from {0} to {1}", _archiveLog.FirstTimestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
					              _archiveLog.LastTimestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));

				zip.Save();
			}

			_archiveLog.Dispose();
			_archiveLog = null;
		}

		public void Dispose()
		{
			if (_archiveLog != null)
			{
				FlushLog();
			}
		}
	}
}
