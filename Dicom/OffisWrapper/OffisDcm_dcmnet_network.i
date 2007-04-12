/////////////////////////////////////////////////////////////////////////
//
// SECTION: Extension of the T_ASC_Network class
//
%typemap(csin) T_ASC_Parameters *associationParameters "getCPtrAndAddReference($csinput)"

%typemap(cscode) T_ASC_Network %{
	// Ensure that the GC does not collect any 
	// T_ASC_Parameters set from C#
	// as the underlying C++ class stores a shallow copy
	private T_ASC_Parameters _parametersReference;
	private HandleRef getCPtrAndAddReference(T_ASC_Parameters associationParameters) {
		_parametersReference = associationParameters;
		return T_ASC_Parameters.getCPtr(associationParameters);
	}
%}

/////////////////////////////////////////////////////////////////////////
// 
// SECTION: Customize the Dispose method to clean up references to the
// C# proxy objects that hold references to underlying unmanaged C++
// objects, created using the getCPtrAndAddXXX methods. The references
// were created in the first place, so that they are not GC when
// PInvoke occurs, but they have to be cleaned up afterwards
//
%typemap(csdestruct, methodname="Dispose") T_ASC_Network {
	if(swigCPtr.Handle != IntPtr.Zero && swigCMemOwn) 
	{
		swigCMemOwn = false;
		$imcall;
		_parametersReference = null;
	}
	swigCPtr = new HandleRef(null, IntPtr.Zero);

	GC.SuppressFinalize(this);
}

%typemap(csvarin) T_ASC_NetworkRole role %{
	set {
		if (swigCPtr.Handle != IntPtr.Zero)
			$imcall;
	}
%}

%typemap(csvarout) T_ASC_NetworkRole role %{
	get {
		if (swigCPtr.Handle != IntPtr.Zero)
			return (T_ASC_NetworkRole) ($imcall);
		else
			throw new System.Exception("swigCPtr is null");
	}
%}

%typemap(csvarin) int acceptorPort %{
	set {
		if (swigCPtr.Handle != IntPtr.Zero)
			$imcall;
	}
%}

struct T_ASC_Network
{
    T_ASC_NetworkRole   role;
    int             	acceptorPort;
    DUL_NETWORKKEY      *network;
};

%extend(canthrow=1) T_ASC_Network {

	T_ASC_Network(T_ASC_NetworkRole role,
					int acceptorPort,
					int timeout) throw (dicom_runtime_error)
	{
		T_ASC_Network* pNetwork = 0;
		OFCondition result = ASC_initializeNetwork(role, 
			acceptorPort, 
			timeout, 
			&pNetwork);

		if (result.bad())
		{
			string msg = string("T_ASC_Network ctor: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}
		
		return pNetwork;
	}

	T_ASC_Association* CreateAssociation(T_ASC_Parameters* associationParameters)
		throw (dicom_runtime_error)
	{
		T_ASC_Association* pAssociation = 0;
		OFCondition result = ASC_requestAssociation(self, 
			associationParameters, 
			&pAssociation); 

		if (result.bad())
		{
			if (result == DUL_ASSOCIATIONREJECTED)
			{
				T_ASC_RejectParameters rej;
				ASC_getRejectParameters(associationParameters, &rej);

				string msg = string("Association rejection ");
								
				switch (rej.result) 
				{
				case ASC_RESULT_REJECTEDPERMANENT:
					msg += "permanent "; 
					break;
				case ASC_RESULT_REJECTEDTRANSIENT:
					msg += "transient "; 
					break;
				default:
					msg += "UNKNOWN ";
					break;
				}

				switch (rej.source) 
				{
				case ASC_SOURCE_SERVICEUSER:
					msg += "from Service User: ";
					break;
				case ASC_SOURCE_SERVICEPROVIDER_ACSE_RELATED:
					msg += "from Service Provider (ACSE Related): ";
					break;
				case ASC_SOURCE_SERVICEPROVIDER_PRESENTATION_RELATED:
					msg += "from Service Provider (Presentation Related): ";
					break;
				default:
					msg += "from UNKNOWN: ";
					break;
				}

				switch (rej.reason) 
				{
				case ASC_REASON_SU_NOREASON:
				case ASC_REASON_SP_ACSE_NOREASON:
					msg += "No reason given";
					break;
				case ASC_REASON_SU_APPCONTEXTNAMENOTSUPPORTED:
					msg += "App Context Name not supported";
					break;
				case ASC_REASON_SU_CALLINGAETITLENOTRECOGNIZED:
					msg += "Calling AE Title not recognized";
					break;
				case ASC_REASON_SU_CALLEDAETITLENOTRECOGNIZED:
					msg += "Called AE Title not recognzed";
					break;
				case ASC_REASON_SP_ACSE_PROTOCOLVERSIONNOTSUPPORTED:
					msg += "Protocol version not supported";
					break;
					/* Service Provider Presentation Related reasons */
				case ASC_REASON_SP_PRES_TEMPORARYCONGESTION:
					msg += "Temporary congestion";
					break;
				case ASC_REASON_SP_PRES_LOCALLIMITEXCEEDED:
					msg += "Local limit exceeded";
					break;
				default:
					msg += "UNKNOWN reason";
					break;
				}
				
				throw dicom_runtime_error(result, msg);
			} 
			else 
			{
				string msg = string("Association request failed: ") + result.text();
				throw dicom_runtime_error(result, msg);
			}
			
		}

		if (0 == ASC_countAcceptedPresentationContexts(associationParameters)) 
		{
			// clean up the allocated association before throwing exception
			ASC_destroyAssociation(&pAssociation);
			throw dicom_runtime_error(result, "T_ASC_Network.CreateAssociation: No acceptable Presentation Contexts");
		}

		return pAssociation;
	}

	
	T_ASC_Association* AcceptAssociation(const char* ownAETitle, int operationTimeout, 
			int currentNumberOfAssociations, int maximumNumberOfAssociations)
		throw (dicom_runtime_error)
	{
		const char* knownAbstractSyntaxes[] =
		{
			UID_VerificationSOPClass,
			UID_FINDStudyRootQueryRetrieveInformationModel,
			UID_MOVEStudyRootQueryRetrieveInformationModel
		};

		const char* transferSyntaxes[] = { NULL, NULL, NULL, NULL };
		int numTransferSyntaxes = 0;

		char buf[BUFSIZ];
		T_ASC_Association* assoc;
		OFCondition cond;

		cond = ASC_receiveAssociation(self, &assoc, ASC_DEFAULTMAXPDU, NULL, NULL, 0, 
				DUL_NOBLOCK, operationTimeout);

		if (cond.bad())
		{
			ASC_dropSCPAssociation(assoc);
			ASC_destroyAssociation(&assoc);

			if (DUL_NOASSOCIATIONREQUEST == cond)
			{
				// this is a special case:
				// indicate that there is no association, that we just timed out
				// waiting for one
				return NULL;
			}
			else
			{
				string msg = string("T_ASC_Network::AcceptAssociation ") + cond.text();
				throw dicom_runtime_error(cond, msg);
			}
		}

		if (currentNumberOfAssociations >= maximumNumberOfAssociations)
		{
			T_ASC_RejectParameters rej =
			{
				ASC_RESULT_REJECTEDTRANSIENT,
				ASC_SOURCE_SERVICEPROVIDER_PRESENTATION_RELATED,
				ASC_REASON_SP_PRES_TEMPORARYCONGESTION
			};

			OFCondition	cond = ASC_rejectAssociation(assoc, &rej);
			
			ASC_dropSCPAssociation(assoc);
			ASC_destroyAssociation(&assoc);

			return NULL;
		}

		// at this point an association has been received
		// We prefer explicit transfer syntaxes.
		// If we are running on a Little Endian machine we prefer
		// LittleEndianExplicitTransferSyntax to BigEndianTransferSyntax.
		if (gLocalByteOrder == EBO_LittleEndian)  // defined in dcxfer.h
		{
			transferSyntaxes[0] = UID_LittleEndianExplicitTransferSyntax;
			transferSyntaxes[1] = UID_BigEndianExplicitTransferSyntax;
		}
		else
		{
			transferSyntaxes[0] = UID_BigEndianExplicitTransferSyntax;
			transferSyntaxes[1] = UID_LittleEndianExplicitTransferSyntax;
		}
		transferSyntaxes[2] = UID_LittleEndianImplicitTransferSyntax;
		numTransferSyntaxes = 3;

		// accept verification sop class
		cond = ASC_acceptContextsWithPreferredTransferSyntaxes(assoc->params, 
				knownAbstractSyntaxes, DIM_OF(knownAbstractSyntaxes), 
				transferSyntaxes, numTransferSyntaxes);

		if (cond.bad())
		{
			ASC_dropSCPAssociation(assoc);
			ASC_destroyAssociation(&assoc);

			string msg = string("T_ASC_Network::AcceptAssociation ") + cond.text();
			throw dicom_runtime_error(cond, msg);
		}

		// accepts all Storage SOP classes defined in dcuid.h that
		// match acceptable transfer syntaxes
		cond = ASC_acceptContextsWithPreferredTransferSyntaxes(assoc->params, 
				dcmAllStorageSOPClassUIDs, numberOfAllDcmStorageSOPClassUIDs, 
				transferSyntaxes, numTransferSyntaxes);

		if (cond.bad())
		{
			ASC_dropSCPAssociation(assoc);
			ASC_destroyAssociation(&assoc);

			string msg = string("T_ASC_Network::AcceptAssociation ") + cond.text();
			throw dicom_runtime_error(cond, msg);
		}

		ASC_setAPTitles(assoc->params, NULL, NULL, ownAETitle);

		cond = ASC_getApplicationContextName(assoc->params, buf);

		if ((cond.bad()) || strcmp(buf, UID_StandardApplicationContext) != 0)
		{
			// reject: the application context name is not supported 
			T_ASC_RejectParameters rej =
			{
				  ASC_RESULT_REJECTEDPERMANENT,
				  ASC_SOURCE_SERVICEUSER,
				  ASC_REASON_SU_APPCONTEXTNAMENOTSUPPORTED
			};

			cond = ASC_rejectAssociation(assoc, &rej);

			ASC_dropSCPAssociation(assoc);
			ASC_destroyAssociation(&assoc);

			return NULL;
		}
		else
		{
			cond = ASC_acknowledgeAssociation(assoc);
			if (cond.bad())
			{
				ASC_dropSCPAssociation(assoc);
				ASC_destroyAssociation(&assoc);

				string msg = string("T_ASC_Network::AcceptAssociation ") + cond.text();
				throw dicom_runtime_error(cond, msg);
			}
		}

		// store calling and called aetitle in global variables to enable
		// the --exec options using them. Enclose in quotation marks because
		// aetitles may contain space characters.
		DIC_AE callingTitle;
		DIC_AE calledTitle;
		ASC_getAPTitles(assoc->params, callingTitle, calledTitle, NULL);
		
		return assoc;
	}
}
