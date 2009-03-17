#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Xml.Serialization;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// Abstract class representing a command within the Command pattern.
	/// </summary>
	/// <remarks>
	/// <para>The Command pattern is used throughout the ImageServer when doing
	/// file and database operations to allow undoing of the operations.  This
	/// abstract class is used as the interface for the command.</para>
	/// </remarks>
	public abstract class ServerCommand: IServerCommand
	{
		#region Private Members
		private string _description;
		private bool _requiresRollback;
		private readonly ServerCommandStatistics _stats;
            
		#endregion

		#region Public property

		/// <summary>
		/// Gets the <see cref="ServerCommandStatistics"/> of the command.
		/// </summary>
		public ServerCommandStatistics Statistics
		{
			get { return _stats; }
		}

		#endregion

		#region Constructor
		/// <summary>
		/// Constructor for a ServerCommand.
		/// </summary>
		/// <param name="description">A description of the command</param>
		/// <param name="requiresRollback">bool telling if the command requires a rollback of the operation if it fails</param>
		public ServerCommand(string description, bool requiresRollback)
		{
			_description = description;
			_requiresRollback = requiresRollback;
			_stats = new ServerCommandStatistics(this);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets and sets a value describing what the command is doing.
		/// </summary>
	    [XmlIgnore]
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Gets a value describing if the ServerCommand requires a rollback of the operation its included in if it fails during execution.
		/// </summary>
        [XmlIgnore]
		public bool RequiresRollback
		{
			get { return _requiresRollback; }
			set { _requiresRollback = value; }
		}
		#endregion

		#region Events
		#endregion

		#region Public Methods
		/// <summary>
		/// Execute the ServerCommand.
		/// </summary>
		public void Execute()
		{
			try
			{
				_stats.Start();
				OnExecute();    
			}
			finally
			{
				_stats.End();
			}
            
		}

        
		/// <summary>
		/// Undo the operation done by <see cref="Execute"/>.
		/// </summary>
		public void Undo()
		{
			OnUndo();
			_stats.End();
		}
		#endregion

		#region Virtual methods

		protected abstract void OnExecute();
		protected abstract void OnUndo();
		#endregion
	}
}