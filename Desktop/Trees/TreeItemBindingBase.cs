#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Base implementation of <see cref="ITreeItemBinding"/>.  Provides null default implementations of most methods.
    /// </summary>
    public abstract class TreeItemBindingBase : ITreeItemBinding
    {
        #region ITreeItemBinding members

        public abstract string GetNodeText(object item);

        public virtual bool GetIsChecked(object item)
        {
            return false;
        }

        public virtual void SetIsChecked(object item, bool value)
        {
            return;
        }

        public virtual string GetTooltipText(object item)
        {
            return null;
        }

        public virtual IconSet GetIconSet(object item)
        {
            return null;
        }

        public virtual IResourceResolver GetResourceResolver(object item)
        {
            return null;
        }
        
        public virtual bool CanHaveSubTree(object item)
        {
            return true;
        }

        public virtual bool ShouldInitiallyExpandSubTree(object item)
        {
            return false;
        }

        public virtual ITree GetSubTree(object item)
        {
            return null;
        }

        public virtual DragDropKind CanAcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return DragDropKind.None;
        }

        public virtual DragDropKind AcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return DragDropKind.None;
        }

        #endregion
    }
}
