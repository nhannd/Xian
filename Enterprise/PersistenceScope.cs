using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Controls the type of peristence context that <see cref="PersistenceScope"/> creates.
    /// </summary>
    public enum PersistenceContextType
    {
        Read,
        Update
    }

    /// <summary>
    /// Possible actions that will be taken on a persistence context when the <see cref="PersistenceScope"/>
    /// terminates.
    /// </summary>
    public enum PersistenceScopeDisposeAction
    {
        /// <summary>
        /// Closes the context and disposes it
        /// </summary>
        Close,

        /// <summary>
        /// Suspends the context
        /// </summary>
        Suspend
    }

    /// <summary>
    /// Controls whether the scope will attempt to inherit an existing persistence context, or create a new one
    /// </summary>
    public enum PersistenceScopeOption
    {
        /// <summary>
        /// Inherits an existing context, or creates a new context if no context exists
        /// </summary>
        Required,

        /// <summary>
        /// Creates a new context
        /// </summary>
        RequiresNew
    }


    

    /// <summary>
    /// Used primarily by the AOP advice classes to manage the scoping of persistence contexts around service method calls.
    /// Can also be used by application code for the same purpose.
    /// 
    /// Semantics of use are similar to the .NET framework <see cref="System.Transactions.TransactionScope"/> class.
    /// </summary>
    // Taken and modified from the following MSDN article by Stephen Toub:
    // http://msdn.microsoft.com/msdnmag/issues/06/09/NETMatters/default.aspx
    public class PersistenceScope : IDisposable
    {
        enum Vote
        {
            Undecided,
            Abort,
            Complete
        }

        private bool _disposed;
        private IPersistenceContext _context;
        private PersistenceScope _parent;
        private Vote _vote;

        private PersistenceScopeDisposeAction _disposeAction;

        [ThreadStatic]
        private static PersistenceScope _head;

        /// <summary>
        /// Creates a new persistence scope for the specified context.  The scope assumes ownership of the context
        /// and closes it when the scope terminates.
        /// </summary>
        /// <param name="context"></param>
        public PersistenceScope(IPersistenceContext context)
            :this(context, PersistenceScopeDisposeAction.Close)
        {
        }

        /// <summary>
        /// Creates a new persistence scope for the specified update context.  The scope assumes ownership of the context
        /// and the specified action will be taken on the context when the scope terminates.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="disposeAction"></param>
        public PersistenceScope(IUpdateContext context, PersistenceScopeDisposeAction disposeAction)
            :this((IPersistenceContext)context, disposeAction)
        {
        }

        /// <summary>
        /// Creates a new persistence scope for the specified context type, inheriting an existing context if possible.
        /// </summary>
        /// <remarks>
        /// If there is no parent scope, a new context of the specified type will be opened.
        /// If there is a parent scope holding a context of the correct type, that context will be inherited.
        /// If a new context is opened, the scope owns the context, and closes it when the scope terminates.
        /// If a context was inherited, the scope does not own the context, and takes no action on it when the scope terminates.
        /// If an update context is requested and while a parent scope is holding a read context, an exception will be thrown.
        /// If a read context is requested while a parent scope is holding an update context, the update context will be inherited.
        /// </remarks>
        /// <param name="contextType"></param>
        public PersistenceScope(PersistenceContextType contextType)
            :this(InheritOrCreateContext(contextType, PersistenceScopeOption.Required), PersistenceScopeDisposeAction.Close)
        {
        }

        /// <summary>
        /// Creates a new persistence scope for the specified context type, using the specified option to determine
        /// whether to create a new persistence context, or attempt to inherit an existing context.
        /// </summary>
        /// <remarks>
        /// If there is no parent scope, or <see cref = "PersistenceScopeOption.RequiresNew"/> was specified, 
        /// a new context of the specified type will be opened.  Otherwise, 
        /// if there is a parent scope holding a context of the correct type, that context will be inherited.
        /// If a new context is opened, the scope owns the context, and closes it when the scope terminates.
        /// If a context was inherited, the scope does not own the context, and takes no action on it when the scope terminates.
        /// If an update context is requested and while a parent scope is holding a read context, an exception will be thrown.
        /// If a read context is requested while a parent scope is holding an update context, the update context will be inherited.
        /// </remarks>
        /// <param name="contextType"></param>
        /// <param name="scopeOption"></param>
        public PersistenceScope(PersistenceContextType contextType, PersistenceScopeOption scopeOption)
            : this(InheritOrCreateContext(contextType, scopeOption), PersistenceScopeDisposeAction.Close)
        {
        }


        /// <summary>
        /// Private constructor, used by public constructors
        /// </summary>
        /// <param name="context"></param>
        /// <param name="disposeAction"></param>
        private PersistenceScope(IPersistenceContext context, PersistenceScopeDisposeAction disposeAction)
        {
            _context = context;
            _disposeAction = disposeAction;

            _parent = _head;
            _head = this;
        }

        private static IPersistenceContext InheritOrCreateContext(PersistenceContextType contextType, PersistenceScopeOption scopeOption)
        {
            if (scopeOption == PersistenceScopeOption.RequiresNew)
            {
                // need to create a new context
                return (contextType == PersistenceContextType.Update) ?
                    (IPersistenceContext)Session.Current.DataStore.OpenUpdateContext(UpdateContextSyncMode.Flush) :
                    (IPersistenceContext)Session.Current.DataStore.OpenReadContext();
            }
            else 
            {
                // create a context if one doesn't exist, or attempt to inherit the existing one

                if (contextType == PersistenceContextType.Update)
                {
                    // if no current context, create an update context
                    if (PersistenceScope.Current == null)
                        return Session.Current.DataStore.OpenUpdateContext(UpdateContextSyncMode.Flush);

                    // if the current context is an update context, inherit
                    if (PersistenceScope.Current is IUpdateContext)
                        return PersistenceScope.Current;

                    // can't ask for an update context when current context is a read context
                    throw new InvalidOperationException(SR.ExceptionIncompatiblePersistenceContext);
                }
                else
                {
                    // if no current context, create a read context
                    if (PersistenceScope.Current == null)
                        return Session.Current.DataStore.OpenReadContext();

                    // otherwise return the current context, regardless of its type
                    // (read operations are allowed to execute in an update context)
                    return PersistenceScope.Current;
                }
            }
        }

        public static IPersistenceContext Current
        {
            get { return _head != null ? _head._context : null; }
        }

        public void Complete()
        {
            if (_vote == Vote.Undecided)
            {

                _vote = Vote.Complete;
            }
            else
            {
                //throw new InvalidOperationException("The vote has already been placed");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (this != _head)
                    throw new InvalidOperationException("Disposed out of order.");

                _head = _parent;

                if(OwnsContext)
                {
                    if (_disposeAction == PersistenceScopeDisposeAction.Close)
                        CloseContext();
                    else
                        SuspendContext();
                }
                else
                {
                    // if the vote is still "undecided", treat it as an abort
                    if (_vote == Vote.Undecided)
                    {
                        _vote = Vote.Abort;

                        // we have an inherited context, so we need to propagate "aborts" up to the parent
                        _parent._vote = Vote.Abort;
                    }
                }
            }
        }

        /// <summary>
        /// This scope owns the context if there is no parent scope, or if there is a parent scope
        /// that holds a different context.
        /// </summary>
        private bool OwnsContext
        {
            get { return _parent == null || _parent._context != this._context; }
        }

        private void SuspendContext()
        {
            System.Diagnostics.Debug.Assert(this.OwnsContext && _context is IUpdateContext);

            IUpdateContext uctx = _context as IUpdateContext;
            if (null != uctx)
            {
                uctx.Suspend();
            }
        }

        private void CloseContext()
        {
            System.Diagnostics.Debug.Assert(this.OwnsContext);

            try 
	        {	
                // if it is an update context and the vote is "complete", then try to commit
                IUpdateContext uctx = _context as IUpdateContext;
                if (null != uctx && _vote == Vote.Complete)
                {
                    uctx.Commit();
                }
	        }
	        finally
	        {
                // in any case, we need to dispose of the context here
                _context.Dispose();
                _context = null;
	        }
        }
    }
}
