using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

using ClearCanvas.HL7;

using Iesi.Collections;
using ClearCanvas.Ris.Services;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Common.Utilities;


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

        EntityRef<HL7QueueItem> SelectedHL7QueueItem { get; }
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

            public EntityRef<HL7QueueItem> SelectedHL7QueueItem
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
                _component.ShowAllItems();
            }

            #endregion
        }

        private HL7QueueItem _selectedHL7QueueItem;
        private event EventHandler _selectedHL7QueueItemChanged;

        private HL7QueueItemTableData _queue;
        private IHL7QueueService _hl7QueueService;

        private IPagingController<HL7QueueItem> _pagingController;
        private PagingActionModel<HL7QueueItem> _pagingActionHandler;

        private ToolSet _toolSet;
        private ClickHandlerDelegate _defaultAction;

        private HL7MessageDirection _direction;
        private HL7MessagePeer _peer;
        private string _type;
        private HL7MessageStatusCode _status;
        private DateTime? _createdOnStart;
        private DateTime? _createdOnEnd;
        private DateTime? _updatedOnStart;
        private DateTime? _updatedOnEnd;

        private bool _directionChecked;
        private bool _peerChecked;
        private bool _typeChecked;
        private bool _statusChecked;

        private HL7MessageDirectionEnumTable _directionChoices;
        private HL7MessagePeerEnumTable _peerChoices;
        private string[] _dummyTypes = { "ADT_A01", "ADT_A02", "ADT_A03", "ADT_A04", "ADT_A05", "ADT_A06", "ADT_A07", "ADT_A08", "ADT_A09", "ADT_A10",
                                         /*"ADT_A11", "ADT_A12", "ADT_A13", "ADT_A14", "ADT_A15", "ADT_A16", "ADT_A17", "ADT_A18", "ADT_A19", "ADT_A20",
                                         "ADT_A21", "ADT_A22", "ADT_A23", "ADT_A24", "ADT_A25", "ADT_A26", "ADT_A27", "ADT_A28", "ADT_A29", "ADT_A30",*/
                                         "ORM_O01"};
        private HL7MessageStatusCodeEnumTable _statusChoices;


        public override void Start()
        {
            base.Start();

            _hl7QueueService = ApplicationContext.GetService<IHL7QueueService>();
            _queue = new HL7QueueItemTableData(_hl7QueueService);

            _pagingController = new PagingController<HL7QueueItem>(
                delegate (SearchCriteria criteria, SearchResultPage page)
                {
                    return _hl7QueueService.GetHL7QueueItems((HL7QueueItemSearchCriteria)criteria, page); 
                }
            );
            _pagingActionHandler = new PagingActionModel<HL7QueueItem>(_pagingController, _queue);

            _directionChoices = _hl7QueueService.GetHL7MessageDirectionEnumTable();
            _peerChoices = _hl7QueueService.GetHL7MessagePeerEnumTable();
            _statusChoices = _hl7QueueService.GetHL7MessageStatusCodeEnumTable();

            _type = _dummyTypes[0];

            _toolSet = new ToolSet(new HL7QueueToolExtensionPoint(), new HL7QueueToolContext(this));
        }

        #region Presentation Model
		        
        #region Direction
        public string Direction
        {
            get { return _directionChoices[_direction].Value; }
            set { _direction = _directionChoices[value].Code; }
        }

        public bool DirectionChecked
        {
            get { return _directionChecked; }
            set { _directionChecked = value; }
        }

        public string[] DirectionChoices
        {
            get { return _directionChoices.Values; }
        }
        #endregion

        #region Peer
        public string Peer
        {
            get { return _peerChoices[_peer].Value; }
            set { _peer = _peerChoices[value].Code; }
        }

        public bool PeerChecked
        {
            get { return _peerChecked; }
            set { _peerChecked = value; }
        }

        public string[] PeerChoices
        {
            get { return _peerChoices.Values; }
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

        public string[] TypeChoices
        {
            get { return _dummyTypes; }
        }
        #endregion

        #region Status
        public string Status
        {
            get { return _statusChoices[_status].Value; }
            set { _status = _statusChoices[value].Code; }
        }

        public bool StatusChecked
        {
            get { return _statusChecked; }
            set { _statusChecked = value; }
        }

        public string[] StatusChoices
        {
            get { return _statusChoices.Values; }
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

        public HL7QueueItemTableData Queue
        {
            get { return _queue; }
        }

        public string Message
        {
            get 
            {
                if (_selectedHL7QueueItem != null)
                    return _selectedHL7QueueItem.Message.Format == HL7MessageFormat.Er7 ? _selectedHL7QueueItem.Message.Text.Replace("\r", "\r\n") : _selectedHL7QueueItem.Message.Text;
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
                node.Merge(_pagingActionHandler);
                return node;
            }
        }

        #endregion

        #region HL7QueueToolContext Helpers

        public EntityRef<HL7QueueItem> SelectedHL7QueueItem
        {
            get { return _selectedHL7QueueItem == null ? null : new EntityRef<HL7QueueItem>(_selectedHL7QueueItem); }
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
            HL7QueueItem queueItem = selection.Item as HL7QueueItem;
            if (queueItem != _selectedHL7QueueItem)
            {
                _selectedHL7QueueItem = queueItem;
                EventsHelper.Fire(_selectedHL7QueueItemChanged, this, EventArgs.Empty);
                NotifyPropertyChanged("Message");
            }
        }

        public void ShowAllItems()
        {
            HL7QueueItemSearchCriteria criteria = new HL7QueueItemSearchCriteria();
            criteria.Status.CreationDateTime.SortAsc(0);

            UpdateTableData(criteria);
        }
        
        public void ShowFilteredItems()
        {
            HL7QueueItemSearchCriteria criteria = new HL7QueueItemSearchCriteria();

            if (_directionChecked)
                criteria.Direction.EqualTo(_direction);
            if (_peerChecked)
                criteria.Message.Peer.EqualTo(_peer);
            if (_typeChecked)
                criteria.Message.MessageType.EqualTo(_type);
            if (_statusChecked)
                criteria.Status.Code.EqualTo(_status);

            if (_createdOnStart.HasValue && _createdOnEnd.HasValue)
                criteria.Status.CreationDateTime.Between(_createdOnStart.Value, _createdOnEnd.Value);
            else if (_createdOnStart.HasValue)
                criteria.Status.CreationDateTime.MoreThanOrEqualTo(_createdOnStart.Value);
            else if (_createdOnEnd.HasValue)
                criteria.Status.CreationDateTime.LessThanOrEqualTo(_createdOnEnd.Value);

            if (_updatedOnStart.HasValue && _updatedOnEnd.HasValue)
                criteria.Status.UpdateDateTime.Between(_updatedOnStart.Value, _updatedOnEnd.Value);
            else if (_updatedOnStart.HasValue)
                criteria.Status.UpdateDateTime.MoreThanOrEqualTo(_updatedOnStart.Value);
            else if (_updatedOnEnd.HasValue)
                criteria.Status.UpdateDateTime.LessThanOrEqualTo(_updatedOnEnd.Value);

            criteria.Status.CreationDateTime.SortAsc(0);

            UpdateTableData(criteria);
        }

        private void UpdateTableData(HL7QueueItemSearchCriteria criteria)
        {
            _queue.Items.Clear();
            _queue.Items.AddRange(_pagingController.GetFirst(criteria));
        }
    }
}
