using System;
using System.Collections.Generic;
using System.Text;

using Iesi.Collections;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.HL7Admin;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.Admin;


namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class HL7QueueToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IHL7QueueToolContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }
        ClickHandlerDelegate DefaultAction { get; set; }

        HL7QueueItemDetail SelectedHL7QueueItem { get; }
        event EventHandler SelectedHL7QueueItemChanged;

        void Refresh();
    }

    /// <summary>
    /// Extension point for views onto <see cref="HL7QueuePreviewComponent"/>
    /// </summary>
    [ExtensionPoint()]
    public class HL7QueuePreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// HL7QueuePreviewComponent
    /// </summary>
    [AssociateView(typeof(HL7QueuePreviewComponentViewExtensionPoint))]
    public class HL7QueuePreviewComponent : ApplicationComponent
    {
        class HL7QueueToolContext : ToolContext, IHL7QueueToolContext
        {
            private HL7QueuePreviewComponent _component;

            public HL7QueueToolContext(HL7QueuePreviewComponent component)
            {
                _component = component;
            }

            #region IHL7QueueToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            public ClickHandlerDelegate DefaultAction
            {
                get { return _component._defaultAction; }
                set { _component._defaultAction = value; }
            }

            public HL7QueueItemDetail SelectedHL7QueueItem
            {
                get { return _component.SelectedHL7QueueItem; }
            }

            public event EventHandler SelectedHL7QueueItemChanged
            {
                add { _component.SelectedHL7QueueItemChanged += value; }
                remove { _component.SelectedHL7QueueItemChanged -= value; }
            }

            public void Refresh()
            {
                if (_component.RefreshFiltered)
                {
                    _component.ShowFilteredItems();
                }
                else 
                {
                    _component.ShowAllItems();
                }
            }

            #endregion
        }

        private HL7QueueItemDetail _selectedHL7QueueItem;
        private event EventHandler _selectedHL7QueueItemChanged;

        private HL7QueueItemSummaryTable _queue;
        private bool _refreshFiltered = false;

        //private IPagingController<HL7QueueItemSummary> _pagingController;
        //private PagingActionModel<HL7QueueItemSummary> _pagingActionHandler;

        private ToolSet _toolSet;
        private ClickHandlerDelegate _defaultAction;

        private EnumValueInfo _direction;
        private EnumValueInfo _peer;
        private string _type;        
        private EnumValueInfo _status;
        private DateTime? _createdOnStart;
        private DateTime? _createdOnEnd;
        private DateTime? _updatedOnStart;
        private DateTime? _updatedOnEnd;

        private bool _directionChecked;
        private bool _peerChecked;
        private bool _typeChecked;
        private bool _statusChecked;

        private List<EnumValueInfo> _directionChoices;
        private List<EnumValueInfo> _peerChoices;
        private List<string> _typeChoices;
        private List<EnumValueInfo> _statusChoices;

        public override void Start()
        {
            base.Start();

            _queue = new HL7QueueItemSummaryTable();

            //TODO: figure out how to replace use of SearchCriteria in PagingController

            //_pagingController = new PagingController<HL7QueueItemSummary>(
            //    delegate (SearchCriteria criteria, SearchResultPage page)
            //    {
            //        Platform.GetService<IHL7QueueService>(
            //            delegate(IHL7QueueService service)
            //            {
            //                try
            //                {
            //                    formResponse = service.GetHL7QueueFormData(formRequest);
            //                }
            //                catch (Exception e)
            //                {
            //                    ExceptionHandler.Report(e, desktopwindow);
            //                }
            //            });
            //        return _hl7QueueService.GetHL7QueueItems((HL7QueueItemSearchCriteria)criteria, page); 
            //    }
            //);
            //_pagingActionHandler = new PagingActionModel<HL7QueueItemSummary>(_pagingController, _queue);

            GetHL7QueueFormDataRequest formRequest = new GetHL7QueueFormDataRequest();
            GetHL7QueueFormDataResponse formResponse = null;

            Platform.GetService<IHL7QueueService>(
                delegate(IHL7QueueService service)
                {
                    try
                    {
                        formResponse = service.GetHL7QueueFormData(formRequest);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, Host.DesktopWindow);
                    }
                });

            if (formResponse != null)
            {
                _directionChoices = formResponse.DirectionChoices;
                _peerChoices = formResponse.PeerChoices;
                _statusChoices = formResponse.StatusCodeChoices;
                _typeChoices = formResponse.MessageTypeChoices;

                _direction = _directionChoices[0];
                _peer = _peerChoices[0];
                _status = _statusChoices[0];
                _type = _typeChoices[0];
            }
            else
            {
                _directionChoices = new List<EnumValueInfo>();
                _peerChoices = new List<EnumValueInfo>();
                _statusChoices = new List<EnumValueInfo>();
                _typeChoices = new List<string>();
            }

            _toolSet = new ToolSet(new HL7QueueToolExtensionPoint(), new HL7QueueToolContext(this));
        }

        #region Presentation Model
		        
        #region Direction
        public string Direction
        {
            get { return _direction.Value; }
            set 
            {
                _direction = string.IsNullOrEmpty(value)
                    ? null
                    : CollectionUtils.SelectFirst<EnumValueInfo>(
                        _directionChoices,
                        delegate(EnumValueInfo info) { return info.Value == value; });
            }
        }

        public bool DirectionChecked
        {
            get { return _directionChecked; }
            set { _directionChecked = value; }
        }

        public IList<string> DirectionChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_directionChoices); }
        }
        #endregion

        #region Peer
        public string Peer
        {
            get { return _peer.Value; }
            set
            {
                _peer = string.IsNullOrEmpty(value)
                    ? null
                    : CollectionUtils.SelectFirst<EnumValueInfo>(
                        _peerChoices,
                        delegate(EnumValueInfo info) { return info.Value == value; });
            }
        }

        public bool PeerChecked
        {
            get { return _peerChecked; }
            set { _peerChecked = value; }
        }

        public IList<string> PeerChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_peerChoices); }
        }
        #endregion

        #region Type
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public bool TypeChecked
        {
            get { return _typeChecked; }
            set { _typeChecked = value; }
        }

        public IList<string> TypeChoices
        {
            get { return _typeChoices.ToArray(); }
        }
        #endregion

        #region Status
        public string Status
        {
            get { return _status.Value; }
            set
            {
                _status = string.IsNullOrEmpty(value)
                    ? null
                    : CollectionUtils.SelectFirst<EnumValueInfo>(
                        _statusChoices,
                        delegate(EnumValueInfo info) { return info.Value == value; });
            }
        }

        public bool StatusChecked
        {
            get { return _statusChecked; }
            set { _statusChecked = value; }
        }

        public IList<string> StatusChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_statusChoices); }
        }
        #endregion

        #region Created On
        public DateTime? CreatedOnStart
        {
            get { return _createdOnStart; }
            set { _createdOnStart = value; }
        }

        public DateTime? CreatedOnEnd
        {
            get { return _createdOnEnd; }
            set { _createdOnEnd = value; }
        }

        #endregion

        #region Updated On
        public DateTime? UpdatedOnStart
        {
            get { return _updatedOnStart; }
            set { _updatedOnStart = value; }
        }

        public DateTime? UpdatedOnEnd
        {
            get { return _updatedOnEnd; }
            set { _updatedOnEnd = value; }
        }

        #endregion

        public HL7QueueItemSummaryTable Queue
        {
            get { return _queue; }
        }

        public string Message
        {
            get 
            {
                if (_selectedHL7QueueItem != null)
                {
                    return _selectedHL7QueueItem.MessageFormat.Code == "Er7" 
                        ? _selectedHL7QueueItem.MessageText.Replace("\r", "\r\n") 
                        : _selectedHL7QueueItem.MessageText;
                }
                else
                    return string.Empty;
            }
        }

        public ActionModelNode MenuModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "hl7Queue-contextmenu", _toolSet.Actions);
            }
        }

        public ActionModelNode ToolbarModel
        {
            get
            {
                ActionModelNode node = ActionModelRoot.CreateModel(this.GetType().FullName, "hl7Queue-toolbar", _toolSet.Actions);
                //TODO: Restore paging
                //node.Merge(_pagingActionHandler);
                return node;
            }
        }

        #endregion

        #region HL7QueueToolContext Helpers

        public HL7QueueItemDetail SelectedHL7QueueItem
        {
            get { return _selectedHL7QueueItem; }
        }

        public bool RefreshFiltered
        {
            get { return _refreshFiltered; }
        }

        public event EventHandler SelectedHL7QueueItemChanged
        {
            add { _selectedHL7QueueItemChanged += value; }
            remove { _selectedHL7QueueItemChanged -= value; }
        }

        public override IActionSet ExportedActions
        {
            get { return _toolSet.Actions; }
        }

        #endregion

        public void SetSelectedItem(ISelection selection)
        {
            HL7QueueItemSummary selectedSummaryTableItem = selection.Item as HL7QueueItemSummary;

            if (selectedSummaryTableItem == null)
            {
                _selectedHL7QueueItem = null;

                EventsHelper.Fire(_selectedHL7QueueItemChanged, this, EventArgs.Empty);
                NotifyPropertyChanged("Message");
            }
            else if (_selectedHL7QueueItem == null 
                || selectedSummaryTableItem.QueueItemRef != _selectedHL7QueueItem.QueueItemRef)
            {
                LoadHL7QueueItemRequest request = new LoadHL7QueueItemRequest(selectedSummaryTableItem.QueueItemRef);
                LoadHL7QueueItemResponse response = null;

                Platform.GetService<IHL7QueueService>(
                     delegate(IHL7QueueService service)
                     {
                         try
                         {
                             response = service.LoadHL7QueueItem(request);
                         }
                         catch (Exception e)
                         {
                             ExceptionHandler.Report(e, Host.DesktopWindow);
                         }
                     });

                if (response != null)
                {
                    _selectedHL7QueueItem = response.QueueItemDetail;

                    EventsHelper.Fire(_selectedHL7QueueItemChanged, this, EventArgs.Empty);
                    NotifyPropertyChanged("Message");
                }
            }
        }

        public void ShowAllItems()
        {
            ListHL7QueueItemsRequest request = new ListHL7QueueItemsRequest();
            UpdateTableData(request);

            _refreshFiltered = false;
        }
        
        public void ShowFilteredItems()
        {
            ListHL7QueueItemsRequest request = new ListHL7QueueItemsRequest();

            request.Direction = _directionChecked ? _direction : null;
            request.Peer = _peerChecked ? _peer : null;
            request.MessageType = _typeChecked ? _type : null;
            request.StatusCode = _statusChecked ? _status : null;

            request.StartingCreationDateTime = _createdOnStart;
            request.EndingCreationDateTime = _createdOnEnd;

            request.StartingUpdateDateTime = _updatedOnStart;
            request.EndingUpdateDateTime = _updatedOnEnd;

            UpdateTableData(request);

            _refreshFiltered = true;
        }

        private void UpdateTableData(ListHL7QueueItemsRequest request)
        {
            ListHL7QueueItemsResponse response = null;

            Platform.GetService<IHL7QueueService>(
                delegate(IHL7QueueService service)
                {
                    try
                    {
                        response = service.ListHL7QueueItems(request);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, Host.DesktopWindow);
                    }
                });

            _queue.Items.Clear();
            if (response != null)
            {
                //TODO: restore paging functionality
                //_queue.Items.AddRange(_pagingController.GetFirst(criteria));
                _queue.Items.AddRange(response.QueueItems);
            }
        }
    }
}
