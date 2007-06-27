using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    	public enum DcmRoleSelection {
		Disabled,
		SCU,
		SCP,
		Both,
		None
	}

	public enum DcmPresContextResult : byte {
		Proposed = 255,
		Accept = 0,
		RejectUser = 1,
		RejectNoReason = 2,
		RejectAbstractSyntaxNotSupported = 3,
		RejectTransferSyntaxesNotSupported = 4
	}

	internal class DcmPresContext {
		#region Private Members
		private byte _pcid;
		private DcmPresContextResult _result;
		private DcmRoleSelection _roles;
		private DicomUid _abstract;
		private List<TransferSyntax> _transfers;
		#endregion

		#region Public Constructor
		public DcmPresContext(byte pcid, DicomUid abstractSyntax) {
			_pcid = pcid;
			_result = DcmPresContextResult.Proposed;
			_roles = DcmRoleSelection.Disabled;
			_abstract = abstractSyntax;
			_transfers = new List<TransferSyntax>();
		}

		internal DcmPresContext(byte pcid, DicomUid abstractSyntax, TransferSyntax transferSyntax, DcmPresContextResult result) {
			_pcid = pcid;
			_result = result;
			_roles = DcmRoleSelection.Disabled;
			_abstract = abstractSyntax;
			_transfers = new List<TransferSyntax>();
			_transfers.Add(transferSyntax);
		}
		#endregion

		#region Public Properties
		public byte ID {
			get { return _pcid; }
		}

		public DcmPresContextResult Result {
			get { return _result; }
		}

		public bool IsRoleSelect {
			get { return _roles != DcmRoleSelection.Disabled; }
		}
		public bool IsSupportScuRole {
			get { return _roles == DcmRoleSelection.SCU || _roles == DcmRoleSelection.Both; }
		}
		public bool IsSupportScpRole {
			get { return _roles == DcmRoleSelection.SCP || _roles == DcmRoleSelection.Both; }
		}

		public DicomUid AbstractSyntax {
			get { return _abstract; }
		}

		public TransferSyntax AcceptedTransferSyntax {
			get {
				if (_transfers.Count > 0)
					return _transfers[0];
				return null;
			}
		}
		#endregion

		#region Public Members
		public void SetResult(DcmPresContextResult result) {
			_result = result;
		}

		public void SetRoleSelect(DcmRoleSelection roles) {
			_roles = roles;
		}
		public DcmRoleSelection GetRoleSelect() {
			return _roles;
		}

		public void AddTransfer(TransferSyntax ts) {
			if (!_transfers.Contains(ts))
				_transfers.Add(ts);
		}

		public void RemoveTransfer(TransferSyntax ts) {
			if (_transfers.Contains(ts))
				_transfers.Remove(ts);
		}

		public void ClearTransfers() {
			_transfers.Clear();
		}

		public IList<TransferSyntax> GetTransfers() {
			return _transfers.AsReadOnly();
		}

		public bool HasTransfer(TransferSyntax ts) {
			return _transfers.Contains(ts);
		}

		public string GetResultDescription() {
			switch (_result) {
			case DcmPresContextResult.Accept:
				return "Accept";
			case DcmPresContextResult.Proposed:
				return "Proposed";
			case DcmPresContextResult.RejectAbstractSyntaxNotSupported:
				return "Reject - Abstract Syntax Not Supported";
			case DcmPresContextResult.RejectNoReason:
				return "Reject - No Reason";
			case DcmPresContextResult.RejectTransferSyntaxesNotSupported:
				return "Reject - Transfer Syntaxes Not Supported";
			case DcmPresContextResult.RejectUser:
				return "Reject - User";
			default:
				return "Unknown";
			}
		}
		#endregion
	}
    public class Association
    {
        
        #region Private Members
        private uint _maxPduLength = 128 * 1024;
        private ApplicationEntity _calledAE;
        private ApplicationEntity _callingAE;

        #endregion

        /// <summary>
        /// The Maximum PDU Length negotiated for the association
        /// </summary>
        public uint MaximumPduLength
        {
            get { return _maxPduLength; }
            set { _maxPduLength = value; }
        }

        public ApplicationEntity CalledApplication
        {
            get { return _calledAE; }
            set { _calledAE = value; }
        }

        public ApplicationEntity CallingApplication
        {
            get { return _callingAE; }
            set { _callingAE = value; }
        }

        		#region Private Members
		private DicomUid _appCtxNm;
		private DicomUid _implClass;
		private string _implVersion;
		private uint _maxPdu;
		private string _calledAe;
		private string _callingAe;
		private SortedList<byte, DcmPresContext> _presContexts;
		#endregion

		#region Public Constructor
		public Association() {
			_maxPdu = 128 * 1024;
			_appCtxNm = DicomUids.DICOMApplicationContextName;
			_implClass = Implementation.ClassUID;
			_implVersion = Implementation.Version;
			_presContexts = new SortedList<byte, DcmPresContext>();
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the Application Context Name.
		/// </summary>
		public DicomUid ApplicationContextName {
			get { return _appCtxNm; }
			set { _appCtxNm = value; }
		}

		/// <summary>
		/// Gets or sets the Implementation Class UID.
		/// </summary>
		public DicomUid ImplementationClass {
			get { return _implClass; }
			set { _implClass = value; }
		}

		/// <summary>
		/// Gets or sets the Implementation Version Name.
		/// </summary>
		public string ImplementationVersion {
			get { return _implVersion; }
			set { _implVersion = value; }
		}

		/// <summary>
		/// Gets or sets the Called AE title.
		/// </summary>
		public string CalledAE {
			get { return _calledAe; }
			set { _calledAe = value; }
		}

		/// <summary>
		/// Gets or sets the Calling AE title.
		/// </summary>
		public string CallingAE {
			get { return _callingAe; }
			set { _callingAe = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds a Presentation Context to the DICOM Associate.
		/// </summary>
		public void AddPresentationContext(byte pcid, DicomUid abstractSyntax) {
			_presContexts.Add(pcid, new DcmPresContext(pcid, abstractSyntax));
		}

		/// <summary>
		/// Adds a Presentation Context to the DICOM Associate.
		/// </summary>
		public byte AddPresentationContext(DicomUid abstractSyntax) {
			byte pcid = 1;
			foreach (byte id in _presContexts.Keys) {
				if (_presContexts[id].AbstractSyntax == abstractSyntax)
					return id;
				if (id >= pcid)
					pcid = (byte)(id + 2);
			}
			AddPresentationContext(pcid, abstractSyntax);
			return pcid;
		}

		/// <summary>
		/// Determines if the specified Presentation Context ID exists.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <returns>True if exists.</returns>
		public bool HasPresentationContextID(byte pcid) {
			return _presContexts.ContainsKey(pcid);
		}

		/// <summary>
		/// Gets a list of the Presentation Context IDs in the DICOM Associate.
		/// </summary>
		public IList<byte> GetPresentationContextIDs() {
			return _presContexts.Keys;
		}

		/// <summary>
		/// Sets the result of the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <param name="result">Result</param>
		public void SetPresentationContextResult(byte pcid, DcmPresContextResult result) {
			GetPresentationContext(pcid).SetResult(result);
		}

		/// <summary>
		/// Gets the result of the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <returns>Result</returns>
		public DcmPresContextResult GetPresentationContextResult(byte pcid) {
			return GetPresentationContext(pcid).Result;
		}

		/// <summary>
		/// Adds a Transfer Syntax to the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <param name="ts">Transfer Syntax</param>
		public void AddTransferSyntax(byte pcid, TransferSyntax ts) {
			GetPresentationContext(pcid).AddTransfer(ts);
		}

		/// <summary>
		/// Gets the number of Transfer Syntaxes in the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <returns>Number of Transfer Syntaxes</returns>
		public int GetTransferSyntaxCount(byte pcid) {
			return GetPresentationContext(pcid).GetTransfers().Count;
		}

		/// <summary>
		/// Gets the Transfer Syntax at the specified index.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <param name="index">Index of Transfer Syntax</param>
		/// <returns>Transfer Syntax</returns>
		public TransferSyntax GetTransferSyntax(byte pcid, int index) {
			return GetPresentationContext(pcid).GetTransfers()[index];
		}

		/// <summary>
		/// Removes a Transfer Syntax from the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <param name="ts">Transfer Syntax</param>
		public void RemoveTransferSyntax(byte pcid, TransferSyntax ts) {
			GetPresentationContext(pcid).RemoveTransfer(ts);
		}

		/// <summary>
		/// Gets the Abstract Syntax for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <returns>Abstract Syntax</returns>
		public DicomUid GetAbstractSyntax(byte pcid) {
			return GetPresentationContext(pcid).AbstractSyntax;
		}

		/// <summary>
		/// Gets the accepted Transfer Syntax for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <returns>Transfer Syntax</returns>
		public TransferSyntax GetAcceptedTransferSyntax(byte pcid) {
			return GetPresentationContext(pcid).AcceptedTransferSyntax;
		}

		public void SetAcceptedTransferSyntax(byte pcid, int index) {
			TransferSyntax ts = GetPresentationContext(pcid).GetTransfers()[index];
			GetPresentationContext(pcid).ClearTransfers();
			GetPresentationContext(pcid).AddTransfer(ts);
		}

		public void SetAcceptedTransferSyntax(byte pcid, TransferSyntax ts) {
			GetPresentationContext(pcid).ClearTransfers();
			GetPresentationContext(pcid).AddTransfer(ts);
		}

		/// <summary>
		/// Finds the Presentation Context with the specified Abstract Syntax.
		/// </summary>
		/// <param name="abstractSyntax">Abstract Syntax</param>
		/// <returns>Presentation Context ID</returns>
		public byte FindAbstractSyntax(DicomUid abstractSyntax) {
			foreach (DcmPresContext ctx in _presContexts.Values) {
				if (ctx.AbstractSyntax == abstractSyntax)
					return ctx.ID;
			}
			return 0;
		}

		/// <summary>
		/// Finds the Presentation Context with the specified Abstract Syntax and Transfer Syntax.
		/// </summary>
		/// <param name="abstractSyntax">Abstract Syntax</param>
		/// <param name="transferSyntax">Transfer Syntax</param>
		/// <returns>Presentation Context ID</returns>
		public byte FindAbstractSyntaxWithTransferSyntax(DicomUid abstractSyntax, TransferSyntax trasferSyntax) {
			foreach (DcmPresContext ctx in _presContexts.Values) {
				if (ctx.AbstractSyntax == abstractSyntax && ctx.HasTransfer(trasferSyntax))
					return ctx.ID;
			}
			return 0;
		}

		/// <summary>
		/// Determines if Role Selection is enabled for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		public bool IsRoleSelect(byte pcid) {
			return GetPresentationContext(pcid).IsRoleSelect;
		}

		/// <summary>
		/// Determines whether the User Role is supported for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		public bool IsSupportScuRole(byte pcid) {
			return GetPresentationContext(pcid).IsSupportScuRole;
		}

		/// <summary>
		/// Determines whether the Provider Role is supported for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		public bool IsSupportScpRole(byte pcid) {
			return GetPresentationContext(pcid).IsSupportScpRole;
		}

		/// <summary>
		/// Enables or disables Role Selection. It also sets the User Role and Provider Role, if enabled, for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <param name="roles">Supported Roles</param>
		public void SetRoleSelect(byte pcid, DcmRoleSelection roles) {
			GetPresentationContext(pcid).SetRoleSelect(roles);
		}
		#endregion

		#region Internal Methods
		internal void AddPresentationContext(byte pcid, DicomUid abstractSyntax, TransferSyntax transferSyntax, DcmPresContextResult result) {
			_presContexts.Add(pcid, new DcmPresContext(pcid, abstractSyntax, transferSyntax, result));
		}

		internal DcmPresContext GetPresentationContext(byte pcid) {
			DcmPresContext ctx = null;
			if (!_presContexts.TryGetValue(pcid, out ctx))
				throw new NetworkException("Invalid Presentaion Context ID");
			return ctx;
		}

		internal IList<DcmPresContext> GetPresentationContexts() {
			return _presContexts.Values;
		}
		#endregion

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Application Context:		{0}\n", _appCtxNm);
			sb.AppendFormat("Implementation Class:		{0}\n", _implClass);
			sb.AppendFormat("Implementation Version:	{0}\n", _implVersion);
			sb.AppendFormat("Maximum PDU Size:			{0}\n", _maxPdu);
			sb.AppendFormat("Called AE Title:			{0}\n", _calledAe);
			sb.AppendFormat("Calling AE Title:			{0}\n", _callingAe);
			sb.AppendFormat("Presentation Contexts:		{0}\n", _presContexts.Count);
			foreach (DcmPresContext pctx in _presContexts.Values) {
				sb.AppendFormat("	Presentation Context {0} [{1}]\n", pctx.ID, pctx.GetResultDescription());
				sb.AppendFormat("		Abstract: {0}\n", (pctx.AbstractSyntax.Type == UidType.Unknown) ?
					pctx.AbstractSyntax.UID : pctx.AbstractSyntax.Description);
				foreach (TransferSyntax ts in pctx.GetTransfers()) {
					sb.AppendFormat("		Transfer: {0}\n", (ts.UID.Type == UidType.Unknown) ?
						ts.UID.UID : ts.UID.Description);
				}
			}
			return sb.ToString();
		}
    }
}
