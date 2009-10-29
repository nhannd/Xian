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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Action model that allows a user to control a <see cref="IPagingController{TItem}"/>.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
    public class PagingActionModel<TItem> : SimpleActionModel
    {
        private readonly IDesktopWindow _desktopWindow;
        private readonly IPagingController<TItem> _controller;
        private readonly Table<TItem> _table;

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="controller"></param>
        ///<param name="table"></param>
        ///<param name="desktopWindow"></param>
        public PagingActionModel(IPagingController<TItem> controller, Table<TItem> table, IDesktopWindow desktopWindow)
            : base(new ResourceResolver(typeof(PagingActionModel<TItem>).Assembly))
        {
            _controller = controller;
            _table = table;
            _desktopWindow = desktopWindow;

			AddAction("Previous", SR.TitlePrevious, "Icons.PreviousPageToolSmall.png");
            AddAction("Next", SR.TitleNext, "Icons.NextPageToolSmall.png");

            Next.SetClickHandler(OnNext);
            Previous.SetClickHandler(OnPrevious);

            Next.Enabled = _controller.HasNext;
            Previous.Enabled = _controller.HasPrevious;  // can't go back from first

            _controller.PageChanged += PageChangedEventHandler;
        }

        private void OnNext()
        {
            ChangePage(_controller.GetNext());
        }

        private void OnPrevious()
        {
            ChangePage(_controller.GetPrevious());
        }

        private void ChangePage(IEnumerable<TItem> results)
        {
            try
            {
                _table.Items.Clear();
                _table.Items.AddRange(results);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, _desktopWindow);
            }
        }

        private void PageChangedEventHandler(object sender, EventArgs args)
        {
            Next.Enabled = _controller.HasNext;
            Previous.Enabled = _controller.HasPrevious;
        }

        private ClickAction Next
        {
            get { return this["Next"]; }
        }

        private ClickAction Previous
        {
            get { return this["Previous"]; }
        }
    }
}
