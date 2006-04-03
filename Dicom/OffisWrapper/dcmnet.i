%module OffisDcm

%{
#include "assoc.h"
#include "dimse.h"
#include "ofcond.h"
#include "cond.h"
#include "diutil.h"
%}

#ifndef params
	#define params parameters
#endif

#ifndef base
	#define base baseclass
#endif

%include "ofcond.h"
%include "dicom.h"
%include "cond.h"

/////////////////////////////////////////////////////////
//
// MACRO: Change the access modifiers of the constructors 
// so that the creator of the object can manually specify
// cMemoryOwn. This is useful when we manually marshal
// DcmDataset pointers into the C# world; we can 
// simply construct a C# DcmDataset proxy object and give
// the constructor the C-pointer of the real object
// and at the same time, set cMemoryOwn appropriately.
// For example, in the query progress callback, the
// underly C++ DcmDataset object exists only on the stack
// and will be deallocated once the callback returns.
// 
// This macro is for base types, rather than derived types
//
%define CONTROLACCESSPUBLIC(type)
%typemap(csbody) type %{
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  public type ## (IntPtr cPtr, bool cMemoryOwn) 
  {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  public static HandleRef getCPtr( ## type obj) 
  {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }
%}
%enddef

CONTROLACCESSPUBLIC(T_DIMSE_C_FindRSP)
CONTROLACCESSPUBLIC(T_DIMSE_C_FindRQ)
CONTROLACCESSPUBLIC(InteropStoreCallbackInfo)
CONTROLACCESSPUBLIC(InteropFindCallbackInfo)
CONTROLACCESSPUBLIC(InteropMoveCallbackInfo)

/////////////////////////////////////////////////////////////////////////
//
// SECTION: Custom Exception
// The following section implements the support for a custom exception
// to be thrown from the C++ code and caught in the C# code
//
%insert(runtime) %{

#include "ofcond.h"
#include <stdexcept>

class dicom_runtime_error : public std::runtime_error
{
public:
	dicom_runtime_error(OFCondition condition, const string& message)
	: runtime_error(message), _condition(condition)
	{
	}

	~dicom_runtime_error() throw()
	{
	}

	OFCondition _condition;
};

// Code to handle throwing of C# DicomRuntimeApplicationException from C/C++ code.
// The equivalent delegate to the callback, CSharpExceptionCallback_t, is DicomRuntimeExceptionDelegate
// and the equivalent dicomRuntimeExceptionCallback instance is dicomRuntimeDelegate
typedef void (SWIGSTDCALL* CSharpExceptionCallback_t)(OFCondition *, const char *);
CSharpExceptionCallback_t dicomRuntimeExceptionCallback = NULL;

extern "C" SWIGEXPORT
void SWIGSTDCALL DicomRuntimeExceptionRegisterCallback(CSharpExceptionCallback_t dicomRuntimeCallback) {
	dicomRuntimeExceptionCallback = dicomRuntimeCallback;
}

// Note that SWIG detects any method calls named starting with
// SWIG_CSharpSetPendingException for warning 845
static void SWIG_CSharpSetPendingExceptionDicomRuntime(OFCondition* pcondition, const char* message) {
	dicomRuntimeExceptionCallback(pcondition, message);
}

// custom define to change "parameters" variable names back to params in C++ world
#define parameters params
%}

%pragma(csharp) imclassimports=%{
using System;
using System.Text;
using System.Runtime.InteropServices;

// Custom C# Exception
public class DicomRuntimeApplicationException : System.ApplicationException {
	public DicomRuntimeApplicationException(OFCondition condition, string message)
	: base(message)
	{
		_condition = condition;
	}

	public System.UInt16 Module
	{
		get
		{
			return _condition.module();
		}
	}

	public System.UInt16 Code
	{
		get
		{
			return _condition.code();
		}
	}

	private OFCondition _condition = null;
}
%}

%pragma(csharp) imclasscode=%{

	class DicomRuntimeExceptionHelper 
	{
		// C# delegate for the C/C++ dicomRuntimeExceptionCallback
		public delegate void DicomRuntimeExceptionDelegate(IntPtr pcondition, string message);
		static DicomRuntimeExceptionDelegate dicomRuntimeDelegate = new DicomRuntimeExceptionDelegate(SetPendingDicomRuntimeException);

		[DllImport("$dllimport", EntryPoint="DicomRuntimeExceptionRegisterCallback")]
		public static extern
		void DicomRuntimeExceptionRegisterCallback(DicomRuntimeExceptionDelegate dicomRuntimeCallback);

		static void SetPendingDicomRuntimeException(IntPtr pcondition, string message) 
		{
			SWIGPendingException.Set(new DicomRuntimeApplicationException(new OFCondition(pcondition, true), message));
		}

		static DicomRuntimeExceptionHelper() 
		{
			DicomRuntimeExceptionRegisterCallback(dicomRuntimeDelegate);
		}
	}
	static DicomRuntimeExceptionHelper exceptionHelper = new DicomRuntimeExceptionHelper();
%}

%typemap(throws, canthrow=1) dicom_runtime_error {
	OFCondition* pcondition = new OFCondition($1._condition);
	SWIG_CSharpSetPendingExceptionDicomRuntime(pcondition, $1.what());
	return $null;
}

%inline
%{
//-------------------------------------------
// Helper function to set the connection
// timeout in the OFFIS tk
//-------------------------------------------
void SetConnectionTimeout(int newTimeout)
{
	dcmConnectionTimeout.set((Sint32) newTimeout);
}

//-------------------------------------------
// Helper function to initialize Winsock
//-------------------------------------------
void InitializeSockets()
{
#ifdef HAVE_WINSOCK_H
    WSAData winSockData;
    /* we need at least version 1.1 */
    WORD winSockVersionNeeded = MAKEWORD( 1, 1 );
    WSAStartup(winSockVersionNeeded, &winSockData);
#endif
}

//-------------------------------------------
// Helper function to cleanup Winsock
//-------------------------------------------
void DeinitializeSockets()
{
#ifdef HAVE_WINSOCK_H
    WSACleanup();
#endif
}

//-------------------------------------------
// Structs for interop; these need to be here
// so that C# code and access them
//-------------------------------------------
struct InteropStoreCallbackInfo
{
	const char* FileName;
	DcmDataset* ImageDataset;
};

struct InteropFindCallbackInfo
{

};

struct InteropMoveCallbackInfo
{

};

%}

/////////////////////////////////////////////////////////////////////////
//
// SECTION: Delegate callbacks
// The following section implements the support for a delegate in C#
// that will be called as a progress callback from the C++ wrapper
//
%{

//----------------------------------------------------------------------
//                                                 Callback structs
//                                                 ---------------------
//-------------------------------------------
// This struct will hold information about
// a particular association that will be
// passed into the find function. When the
// find function calls the progress callback
// this data will be made available to the
// callback to deal with appropriately
//-------------------------------------------
struct QueryRetrieveCallbackInfo {
    T_ASC_Association *assoc;
    T_ASC_PresentationContextID presId;
};

struct StoreCallbackData
{
  char* imageFileName;
  DcmFileFormat* dcmff;
  T_ASC_Association* assoc;
};

struct FindCallbackData
{
	DIC_US priorStatus;
	DIC_AE ourAETitle;
};

struct MoveCallbackData
{
	DIC_US priorStatus;
	DIC_AE ourAETitle;
};

//-------------------------------------------
// This is the C++ callback function that
// will take the callback progress data and
// translate it into a fashion that it can
// be understood by the C# code
//-------------------------------------------

typedef void (SWIGSTDCALL* QueryCallbackHelperCallback)(void *, 
														T_DIMSE_C_FindRQ *,
														int,
														T_DIMSE_C_FindRSP *,
														DcmDataset *);
static QueryCallbackHelperCallback CSharpQueryCallbackHelperCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterQueryCallbackHelper_OffisDcm(QueryCallbackHelperCallback callback) {
	CSharpQueryCallbackHelperCallback = callback;
}
//-------------------------------------------

//-----------------------------------
typedef void (SWIGSTDCALL* StoreCallbackHelperCallback)(InteropStoreCallbackInfo*);
static StoreCallbackHelperCallback CSharpStoreCallbackHelperCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterStoreCallbackHelper_OffisDcm(StoreCallbackHelperCallback callback) {
	CSharpStoreCallbackHelperCallback = callback;
}
//-----------------------------------

//--------------------------------------------------------------
typedef void (SWIGSTDCALL* FindCallbackHelperCallback)(InteropFindCallbackInfo*);
static FindCallbackHelperCallback CSharpFindCallbackHelperCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterFindCallbackHelper_OffisDcm(FindCallbackHelperCallback callback) {
	CSharpFindCallbackHelperCallback = callback;
}
//--------------------------------------------------------------

//--------------------------------------------------------------
typedef void (SWIGSTDCALL* MoveCallbackHelperCallback)(InteropMoveCallbackInfo*);
static MoveCallbackHelperCallback CSharpMoveCallbackHelperCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterMoveCallbackHelper_OffisDcm(MoveCallbackHelperCallback callback) {
	CSharpMoveCallbackHelperCallback = callback;
}
//--------------------------------------------------------------
static void
addRetreiveAETitle(DcmDataset *rspIds, DIC_AE ourAETitle)
{
    /*
     * Since images are stored only by us (for RSNA'93 demo),
     * we must add in our AE Title to the response identifiers.
     * The DB cannot do this since it does not know our AE Title.
     */
    OFBool ok;

    ok = DU_putStringDOElement(rspIds, DCM_RetrieveAETitle, ourAETitle);
}

static void CFindProgressCallback(
        void *callbackData,
        T_DIMSE_C_FindRQ *request,
        int responseCount,
        T_DIMSE_C_FindRSP *rsp,
        DcmDataset *responseIdentifiers
        )
{
	if (NULL != CSharpQueryCallbackHelperCallback)
	{
		CSharpQueryCallbackHelperCallback(callbackData,
											request,
											responseCount,
											rsp,
											responseIdentifiers);
	}
}

static void CMoveProgressCallback(void *callbackData, 
		T_DIMSE_C_MoveRQ *request,
    	int responseCount, 
		T_DIMSE_C_MoveRSP *response)
{
    OFCondition cond = EC_Normal;
    QueryRetrieveCallbackInfo *myCallbackData;

    myCallbackData = (QueryRetrieveCallbackInfo*)callbackData;
}

static void StoreScpCallback(
    /* in */
    void *callbackData,
    T_DIMSE_StoreProgress *progress,    /* progress state */
    T_DIMSE_C_StoreRQ *req,             /* original store request */
    char *imageFileName, DcmDataset **imageDataSet, /* being received into */
    /* out */
    T_DIMSE_C_StoreRSP *rsp,            /* final store response */
    DcmDataset **statusDetail)
{
    DIC_UI sopClass;
    DIC_UI sopInstance;

	if (progress->state == DIMSE_StoreEnd)
	{
		*statusDetail = NULL;    /* no status detail */

		/* could save the image somewhere else, put it in database, etc */
		/*
		* An appropriate status code is already set in the resp structure, it need not be success.
		* For example, if the caller has already detected an out of resources problem then the
		* status will reflect this.  The callback function is still called to allow cleanup.
		*/
		// rsp->DimseStatus = STATUS_Success;

		if ((imageDataSet)&&(*imageDataSet))
		{
			StoreCallbackData *cbdata = (StoreCallbackData*) callbackData;
			const char* fileName = cbdata->imageFileName;

			E_TransferSyntax xfer = (*imageDataSet)->getOriginalXfer();

			OFCondition cond = cbdata->dcmff->saveFile(fileName, xfer, EET_ExplicitLength, EGL_recalcGL,
				EPD_withoutPadding, (Uint32)0, (Uint32)0, false);

			if (cond.bad())
			{
				rsp->DimseStatus = STATUS_STORE_Refused_OutOfResources;
			}

			/* should really check the image to make sure it is consistent,
			* that its sopClass and sopInstance correspond with those in
			* the request.
			*/
			if ((rsp->DimseStatus == STATUS_Success))
			{
				/* which SOP class and SOP instance ? */
				if (! DU_findSOPClassAndInstanceInDataSet(*imageDataSet, sopClass, sopInstance, true))
				{
					rsp->DimseStatus = STATUS_STORE_Error_CannotUnderstand;
				}
				else if (strcmp(sopClass, req->AffectedSOPClassUID) != 0)
				{
					rsp->DimseStatus = STATUS_STORE_Error_DataSetDoesNotMatchSOPClass;
				}
				else if (strcmp(sopInstance, req->AffectedSOPInstanceUID) != 0)
				{
					rsp->DimseStatus = STATUS_STORE_Error_DataSetDoesNotMatchSOPClass;
				}
			}

			// should fire off image received event
			if (NULL != CSharpStoreCallbackHelperCallback)
			{
				InteropStoreCallbackInfo info;

				// prepare the transmission of data 
				bzero((char*)&info, sizeof(info));
				info.FileName = fileName;
				info.ImageDataset = (*imageDataSet);

				CSharpStoreCallbackHelperCallback(&info);
			}
		}
	}
    return;
}

static OFCondition StoreScp(
	T_ASC_Association *assoc,
	T_DIMSE_Message *msg,
	T_ASC_PresentationContextID presID,
	const char* saveDirectoryPath)
{
	OFCondition cond = EC_Normal;
	T_DIMSE_C_StoreRQ *req;
	char imageFileName[2048];

	req = &msg->msg.CStoreRQ;

	sprintf(imageFileName, "%s%s.%s.dcm",
		saveDirectoryPath,
		dcmSOPClassUIDToModality(req->AffectedSOPClassUID),
		req->AffectedSOPInstanceUID);

	StoreCallbackData callbackData;
	callbackData.assoc = assoc;
	callbackData.imageFileName = imageFileName;
	DcmFileFormat dcmff;
	callbackData.dcmff = &dcmff;

	DcmDataset *dset = dcmff.getDataset();

	cond = DIMSE_storeProvider(assoc, presID, req, (char *)NULL, true,
		&dset, StoreScpCallback, (void*)&callbackData, DIMSE_BLOCKING, 0);

	if (cond.bad())
		if (strcmp(imageFileName, NULL_DEVICE_NAME) != 0) unlink(imageFileName);

    return cond;
}

static void MoveScpCallback(
	/* in */ 
	void *callbackData,  
	OFBool cancelled, T_DIMSE_C_MoveRQ *request, 
	DcmDataset *requestIdentifiers, int responseCount,
	/* out */
	T_DIMSE_C_MoveRSP *response,
	DcmDataset **responseIdentifiers,
	DcmDataset **statusDetail)
{
    OFCondition dbcond = EC_Normal;
    MoveCallbackData *context;

    context = (MoveCallbackData*)callbackData;	/* recover context */

    if (responseCount == 1) 
	{
        // start the database search 
		// dbcond = DB_startFindRequest(context->dbHandle, STATUS_FIND_Refused_OutOfResources
		//		request->AffectedSOPClassUID, requestIdentifiers, &dbStatus);

    }
    
    // cancel was requested, cancel the find
    if (cancelled) 
	{

    }

    if (DICOM_PENDING_STATUS(context->priorStatus)) 
	{
		// find the next matching response
		//dbcond = DB_nextFindResponse(context->dbHandle,
		//		responseIdentifiers, &dbStatus);
		//
		
		// should fire off image received event
		if (NULL != CSharpMoveCallbackHelperCallback)
		{
			InteropMoveCallbackInfo info;
			bzero((char*)&info, sizeof(info));

			// prepare the transmission of data 

			CSharpMoveCallbackHelperCallback(&info);
		}
		else
		{
			*responseIdentifiers = NULL;
			response->DimseStatus = STATUS_MOVE_Refused_OutOfResourcesNumberOfMatches;
			*statusDetail = new DcmDataset();
			(*statusDetail)->putAndInsertString(DCM_ErrorComment, "User-defined callback function for C-MOVE missing.");
			return;
		}
    }

    if (*responseIdentifiers != NULL) 
	{
		addRetreiveAETitle(*responseIdentifiers, context->ourAETitle);
    }

	// set the response status, i.e. whether there are more results 
	// and the status detail
    // response->DimseStatus = dbStatus.status;
    // *statusDetail = dbStatus.statusDetail;

}

static OFCondition MoveScp(T_ASC_Association * assoc, T_DIMSE_C_MoveRQ * request,
	T_ASC_PresentationContextID presID)

{
	OFCondition cond = EC_Normal;
	MoveCallbackData context;

	context.priorStatus = STATUS_Pending;
	ASC_getAPTitles(assoc->params, NULL, context.ourAETitle, NULL);

	cond = DIMSE_moveProvider(assoc, presID, request, 
		MoveScpCallback, &context, DIMSE_BLOCKING, 0);

	return cond; 
}

static void FindScpCallback(
	/* in */ 
	void *callbackData,  
	OFBool cancelled, T_DIMSE_C_FindRQ *request, 
	DcmDataset *requestIdentifiers, int responseCount,
	/* out */
	T_DIMSE_C_FindRSP *response,
	DcmDataset **responseIdentifiers,
	DcmDataset **statusDetail)
{
    OFCondition dbcond = EC_Normal;
    FindCallbackData *context;

    context = (FindCallbackData*)callbackData;	/* recover context */

    if (responseCount == 1) 
	{
        // start the database search 
		// dbcond = DB_startFindRequest(context->dbHandle, STATUS_FIND_Refused_OutOfResources
		//		request->AffectedSOPClassUID, requestIdentifiers, &dbStatus);

    }
    
    // cancel was requested, cancel the find
    if (cancelled) 
	{

    }

    if (DICOM_PENDING_STATUS(context->priorStatus)) 
	{
		// find the next matching response
		//dbcond = DB_nextFindResponse(context->dbHandle,
		//		responseIdentifiers, &dbStatus);
		//
		
		// should fire off image received event
		if (NULL != CSharpMoveCallbackHelperCallback)
		{
			InteropFindCallbackInfo info;
			bzero((char*)&info, sizeof(info));

			// prepare the transmission of data 

			CSharpFindCallbackHelperCallback(&info);
		}
		else
		{
			*responseIdentifiers = NULL;
			response->DimseStatus = STATUS_FIND_Refused_OutOfResources;
			*statusDetail = new DcmDataset();
			(*statusDetail)->putAndInsertString(DCM_ErrorComment, "User-defined callback function for C-FIND missing.");
			return;
		}
    }

    if (*responseIdentifiers != NULL) 
	{
		addRetreiveAETitle(*responseIdentifiers, context->ourAETitle);
    }

	// set the response status, i.e. whether there are more results 
	// and the status detail
    // response->DimseStatus = dbStatus.status;
    // *statusDetail = dbStatus.statusDetail;

}

static OFCondition FindScp(T_ASC_Association * assoc, T_DIMSE_C_FindRQ * request,
	T_ASC_PresentationContextID presID)

{
	OFCondition cond = EC_Normal;
	FindCallbackData context;

	context.priorStatus = STATUS_Pending;
	ASC_getAPTitles(assoc->params, NULL, context.ourAETitle, NULL);

	cond = DIMSE_findProvider(assoc, presID, request, 
		FindScpCallback, &context, DIMSE_BLOCKING, 0);

	return cond; 
}

static OFCondition EchoScp(
	T_ASC_Association * assoc,
	T_DIMSE_Message * msg,
	T_ASC_PresentationContextID presID)
{
	/* the echo succeeded !! */
	OFCondition cond = DIMSE_sendEchoResponse(assoc, presID, &msg->msg.CEchoRQ, STATUS_Success, NULL);
	return cond;
}

static OFCondition AcceptSubAssociation(T_ASC_Network * aNet, T_ASC_Association ** assoc)
{
    const char* knownAbstractSyntaxes[] = {
        UID_VerificationSOPClass
    };
    const char* transferSyntaxes[] = { NULL, NULL, NULL, NULL };
    int numTransferSyntaxes;

    OFCondition cond = ASC_receiveAssociation(aNet, assoc, ASC_DEFAULTMAXPDU);
    if (cond.good())
    {
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

        /* accept the Verification SOP Class if presented */
        cond = ASC_acceptContextsWithPreferredTransferSyntaxes(
            (*assoc)->params,
            knownAbstractSyntaxes, DIM_OF(knownAbstractSyntaxes),
            transferSyntaxes, numTransferSyntaxes);

        if (cond.good())
        {
            /* the array of Storage SOP Class UIDs comes from dcuid.h */
            cond = ASC_acceptContextsWithPreferredTransferSyntaxes(
                (*assoc)->params,
                dcmStorageSOPClassUIDs, numberOfDcmStorageSOPClassUIDs,
                transferSyntaxes, numTransferSyntaxes);
        }
    }
	
    if (cond.good()) cond = ASC_acknowledgeAssociation(*assoc);
    if (cond.bad()) 
	{
        ASC_dropAssociation(*assoc);
        ASC_destroyAssociation(assoc);
    }
    return cond;
}

static OFCondition SubOpScp(T_ASC_Association **subAssoc, const char* saveDirectoryPath)
{
    T_DIMSE_Message     msg;
    T_ASC_PresentationContextID presID;

    if (!ASC_dataWaiting(*subAssoc, 0)) /* just in case */
        return DIMSE_NODATAAVAILABLE;

    OFCondition cond = DIMSE_receiveCommand(*subAssoc, DIMSE_BLOCKING, 0, &presID,
            &msg, NULL);

    if (cond == EC_Normal) {
        switch (msg.CommandField) {
        case DIMSE_C_STORE_RQ:
            cond = StoreScp(*subAssoc, &msg, presID, saveDirectoryPath);
            break;
        case DIMSE_C_ECHO_RQ:
            cond = EchoScp(*subAssoc, &msg, presID);
            break;
        default:
            cond = DIMSE_BADCOMMANDTYPE;
            break;
        }
    }
    /* clean up on association termination */
    if (cond == DUL_PEERREQUESTEDRELEASE)
    {
        cond = ASC_acknowledgeRelease(*subAssoc);
        ASC_dropSCPAssociation(*subAssoc);
        ASC_destroyAssociation(subAssoc);
        return cond;
    }
    else if (cond == DUL_PEERABORTEDASSOCIATION)
    {
    }
    else if (cond != EC_Normal)
    {
        /* some kind of error so abort the association */
        cond = ASC_abortAssociation(*subAssoc);
    }

    if (cond != EC_Normal)
    {
        ASC_dropAssociation(*subAssoc);
        ASC_destroyAssociation(subAssoc);
    }
    return cond;
}

static void
CStoreSubOpCallback(void * subOpCallbackData,
        T_ASC_Network *aNet, T_ASC_Association **subAssoc)
{

    if (aNet == NULL) return;   /* help no net ! */

	const char* saveDirectoryPath = (const char*) subOpCallbackData;

    if (*subAssoc == NULL) {
        /* negotiate association */
        AcceptSubAssociation(aNet, subAssoc);
    } else {
        /* be a service class provider */
        SubOpScp(subAssoc, saveDirectoryPath);
    }
}

%}

/////////////////////////////////////////////////////////////////////////
//
// SECTION: Extension of the T_ASC_Network class
//
%typemap(csin) T_ASC_Parameters *associationParameters "getCPtrAndAddReference($csinput)"

%typemap(cscode) T_ASC_Network %{
	// Ensure that the GC doesn't collect any 
	// T_ASC_Parameters set from C#
	// as the underlying C++ class stores a shallow copy
	private T_ASC_Parameters parametersReference;
	private HandleRef getCPtrAndAddReference(T_ASC_Parameters associationParameters) {
		parametersReference = associationParameters;
		return T_ASC_Parameters.getCPtr(associationParameters);
	}
%}


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
				dcmStorageSOPClassUIDs, numberOfDcmStorageSOPClassUIDs, 
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


	~T_ASC_Network() 
	{
		ASC_dropNetwork(&self);
	}
}

/////////////////////////////////////////////////////////////////////////
//
// SECTION: Extension of the T_ASC_Parameters class
//
struct T_ASC_Parameters
{
    DIC_UI ourImplementationClassUID;
    DIC_SH ourImplementationVersionName;
    DIC_UI theirImplementationClassUID;
    DIC_SH theirImplementationVersionName;
    DUL_ModeCallback *modeCallback;

    DUL_ASSOCIATESERVICEPARAMETERS DULparams;
    /*
     * DICOM Upper Layer service parameters.  They should only be
     * accessed via functions defined below. 
     */

    long ourMaxPDUReceiveSize;		/* we say what we can receive */
    long theirMaxPDUReceiveSize;	/* they say what we can send */

};

%extend(canthrow=1) T_ASC_Parameters {

	T_ASC_Parameters(int maxReceivePduLength,
	const char* ourAETitle,
	const char* peerAETitle,
	const char* peerHostName,
	int peerPort) throw (dicom_runtime_error)
	{
		T_ASC_Parameters* pParameters = 0;
		OFCondition result = ASC_createAssociationParameters(&pParameters,
			maxReceivePduLength);

		if (result.bad())
		{
			string msg = string("ASC_createAssociationParameters: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}

		result = ASC_setAPTitles(pParameters, ourAETitle, peerAETitle, NULL);

		if (result.bad())
		{
			string msg = string("ASC_setAPTitles: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}
		
		// we will use an unsecured transport layer at this point (False)
		result = ASC_setTransportLayerType(pParameters, OFFalse);

		if (result.bad())
		{
			string msg = string("ASC_setTransportLayerType: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}

		DIC_NODENAME localHost;
		DIC_NODENAME peerHost;

		gethostname(localHost, sizeof(localHost) - 1);
		sprintf(peerHost, "%s:%d", peerHostName, (int) peerPort);
		result = ASC_setPresentationAddresses(pParameters, localHost, peerHost);

		if (result.bad())
		{
			string msg = string("ASC_setPresentationAddresses: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}

		return pParameters;
	}

	void ConfigureForVerification()
	{
		static const char* transferSyntaxes[] = {
			UID_LittleEndianExplicitTransferSyntax,
			UID_BigEndianExplicitTransferSyntax,
			UID_LittleEndianImplicitTransferSyntax, 
		};

		OFCondition result = ASC_addPresentationContext(
			self, 
			1,
			UID_VerificationSOPClass, 
			transferSyntaxes, 
			DIM_OF(transferSyntaxes));

		if (result.bad())
		{
			string msg = string("ASC_addPresentationContext: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}

	}

	void ConfigureForStudyRootQuery()
	{
		const char* transferSyntaxes[] = {
			UID_LittleEndianExplicitTransferSyntax,
			UID_BigEndianExplicitTransferSyntax,
			UID_LittleEndianImplicitTransferSyntax
		};

		OFCondition result = ASC_addPresentationContext(
			self, 
			1,
			UID_FINDStudyRootQueryRetrieveInformationModel,
			transferSyntaxes, 
			DIM_OF(transferSyntaxes));

		if (result.bad())
		{
			string msg = string("ASC_addPresentationContext: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}
	}

	void ConfigureForCMoveStudyRootQuery()
	{
		const char* transferSyntaxes[] = { NULL, NULL, NULL };
		int numTransferSyntaxes = 0;

        if (gLocalByteOrder == EBO_LittleEndian)  /* defined in dcxfer.h */
        {
            transferSyntaxes[0] = UID_LittleEndianExplicitTransferSyntax;
            transferSyntaxes[1] = UID_BigEndianExplicitTransferSyntax;
        } else {
            transferSyntaxes[0] = UID_BigEndianExplicitTransferSyntax;
            transferSyntaxes[1] = UID_LittleEndianExplicitTransferSyntax;
        }
        transferSyntaxes[2] = UID_LittleEndianImplicitTransferSyntax;
        numTransferSyntaxes = 3;

		OFCondition result = ASC_addPresentationContext(
			self, 
			1,
			UID_MOVEStudyRootQueryRetrieveInformationModel,
			transferSyntaxes, 
			DIM_OF(transferSyntaxes));

		if (result.bad())
		{
			string msg = string("ASC_addPresentationContext: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}
	}

	~T_ASC_Parameters() 
	{
		if (NULL != self)
			ASC_destroyAssociationParameters(&self);
	}
}

/////////////////////////////////////////////////////////////////////////
//
// SECTION: Extension of the T_ASC_Association class
//
%typemap(csin) DcmDataset *cFindDataset "getCPtrAndAddReferenceFind($csinput)"
%typemap(csin) DcmDataset *cMoveDataset "getCPtrAndAddReferenceMove($csinput)"
%typemap(csin) T_ASC_Network *network "getCPtrAndAddReferenceNetwork($csinput)"

%typemap(cscode) T_ASC_Association %{
	// Ensure that the GC doesn't collect any 
	// DcmDataset set from C#
	// as the underlying C++ class stores a shallow copy
	private DcmDataset findReference;
	private DcmDataset moveReference;
	private T_ASC_Network networkReference;
	private HandleRef getCPtrAndAddReferenceFind(DcmDataset cFindDataset) {
		findReference = cFindDataset;
		return DcmDataset.getCPtr(cFindDataset);
	}
	private HandleRef getCPtrAndAddReferenceMove(DcmDataset cMoveDataset) {
		moveReference = cMoveDataset;
		return DcmDataset.getCPtr(cMoveDataset);
	}
	private HandleRef getCPtrAndAddReferenceNetwork(T_ASC_Network network) {
		networkReference = network;
		return T_ASC_Network.getCPtr(network);
	}
%}

struct T_ASC_Association
{
    DUL_ASSOCIATIONKEY *DULassociation;
    T_ASC_Parameters *params;

    unsigned short nextMsgID;	        /* should be incremented by user */
    unsigned long sendPDVLength;	/* max length of PDV to send out */
    unsigned char *sendPDVBuffer;	/* buffer of size sendPDVLength */
};

%typemap(csin) T_ASC_Network *network;

%extend(canthrow=1) T_ASC_Association {

	bool SendCEcho(int numberOfCEchoRepeats) throw (dicom_runtime_error)
	{
		OFCondition cond = EC_Normal;
		unsigned long n = numberOfCEchoRepeats;

		// as long as no error occured and the counter does not equal 0 
		// send an C-ECHO-RQ and handle the response 
		while (cond == EC_Normal && n--) // compare with EC_Normal since DUL_PEERREQUESTEDRELEASE is also good()
		{
			DIC_US msgId = self->nextMsgID++;
			DIC_US status;
			DcmDataset *statusDetail = NULL;

			// send C-ECHO-RQ and handle response 
			cond = DIMSE_echoUser(self, msgId, DIMSE_BLOCKING, 0,
				&status, &statusDetail);

			// check for status detail information, there should never be any 
			if (statusDetail != NULL) {
				delete statusDetail;
			}
		}

		if (cond != EC_Normal && cond.bad())
		{
			string msg = string("SendCEcho: ") + cond.text();

			if (cond != DUL_PEERABORTEDASSOCIATION)
				ASC_abortAssociation(self);
			
			throw dicom_runtime_error(cond, msg);
		}

		return true;
	}

	bool SendCFindStudyRootQuery(DcmDataset* cFindDataset) throw (dicom_runtime_error)
	{
		DIC_US msgId = self->nextMsgID++;
		T_ASC_PresentationContextID presId;
		T_DIMSE_C_FindRQ req;
		T_DIMSE_C_FindRSP rsp;
		DcmDataset *statusDetail = NULL;
		QueryRetrieveCallbackInfo callbackData;

		presId = ASC_findAcceptedPresentationContextID(self, UID_FINDStudyRootQueryRetrieveInformationModel);

		if (presId == 0) 
			throw dicom_runtime_error(DIMSE_NOVALIDPRESENTATIONCONTEXTID, "SendCFindStudyRootQuery: No presentation context");

		// prepare the transmission of data 
		bzero((char*)&req, sizeof(req));
		req.MessageID = msgId;
		strcpy(req.AffectedSOPClassUID, UID_FINDStudyRootQueryRetrieveInformationModel);
		req.DataSetType = DIMSE_DATASET_PRESENT;
		req.Priority = DIMSE_PRIORITY_LOW;

		// prepare the callback 
		callbackData.assoc = self;
		callbackData.presId = presId;

		// finally conduct transmission of data
		OFCondition cond = DIMSE_findUser(self, presId, &req, cFindDataset,
						  CFindProgressCallback, &callbackData,
						  DIMSE_BLOCKING, 0,
						  &rsp, &statusDetail);

		if (cond == EC_Normal)
		{
			return true;
		}
		else if (cond == DUL_PEERREQUESTEDRELEASE)
		{
			ASC_abortAssociation(self);

			string msg = string("SendCFindStudyRootQuery: Protocol error, peer requested release (association aborted); ") + cond.text();
			throw dicom_runtime_error(cond, msg);
		}
		else if (cond == DUL_PEERABORTEDASSOCIATION)
		{
			// right now we don't do anything special
			// but in the future, this could be logged
			return false;
		}
		else // some other abnormal condition
		{
			return false;
		}
		
		/* This was in the original findscu.cxx code and may become
		 * helpful when we do more logging so I'm keeping it here for now

		if (cond == EC_Normal) {
			if (opt_verbose) {
				DIMSE_printCFindRSP(stdout, &rsp);
			} else {
				if (rsp.DimseStatus != STATUS_Success) {
					printf("Response: %s\n", DU_cfindStatusString(rsp.DimseStatus));
				}
			}
		} else {
			if (fname) {
				errmsg("Find Failed, file: %s:", fname);
			} else {
				errmsg("Find Failed, query keys:");
				dcmff.getDataset()->print(COUT);
			}
			DimseCondition::dump(cond);
		}

		if (statusDetail != NULL) {
			printf("  Status Detail:\n");
			statusDetail->print(COUT);
			delete statusDetail;
		}
		*/
	}

	bool SendCMoveStudyRootQuery(DcmDataset* cMoveDataset, T_ASC_Network* network, const char* saveDirectory) throw (dicom_runtime_error)
	{
		T_ASC_PresentationContextID presId;
		T_DIMSE_C_MoveRQ    req;
		T_DIMSE_C_MoveRSP   rsp;
		DIC_US              msgId = self->nextMsgID++;
		const char          *sopClass;
		DcmDataset          *rspIds = NULL;
		DcmDataset          *statusDetail = NULL;
		QueryRetrieveCallbackInfo      callbackData;

		/* which presentation context should be used */
		sopClass = UID_MOVEStudyRootQueryRetrieveInformationModel;
		presId = ASC_findAcceptedPresentationContextID(self, sopClass);
		if (presId == 0) 
			throw dicom_runtime_error(DIMSE_NOVALIDPRESENTATIONCONTEXTID, "SendCMoveStudyRootQuery: No presentation context");

		callbackData.assoc = self;
		callbackData.presId = presId;

		req.MessageID = msgId;
		strcpy(req.AffectedSOPClassUID, sopClass);
		req.Priority = DIMSE_PRIORITY_MEDIUM;
		req.DataSetType = DIMSE_DATASET_PRESENT;
		ASC_getAPTitles(self->params, req.MoveDestination, NULL, NULL);

		OFCondition cond = DIMSE_moveUser(self, presId, &req, cMoveDataset,
			CMoveProgressCallback, &callbackData, DIMSE_BLOCKING, 0,
			network, CStoreSubOpCallback, (void*) saveDirectory,
			&rsp, &statusDetail, &rspIds);

		if (rspIds != NULL) delete rspIds;

		if (cond == EC_Normal)
		{
			return true;
		}
		else if (cond == DUL_PEERREQUESTEDRELEASE)
		{
			ASC_abortAssociation(self);

			string msg = string("SendCFindStudyRootQuery: Protocol error, peer requested release (association aborted); ") + cond.text();
			throw dicom_runtime_error(cond, msg);
		}
		else if (cond == DUL_PEERABORTEDASSOCIATION)
		{
			// right now we don't do anything special
			// but in the future, this could be logged
			return false;
		}
		else // some other abnormal condition
		{
			return false;
		}
	}

	void DropAssociation()
	{
		OFCondition cond = ASC_dropSCPAssociation(self);
	}

	void RespondToReleaseRequest()
	{
		OFCondition cond = ASC_acknowledgeRelease(self);
        cond = ASC_dropSCPAssociation(self);
	}

	OFCondition ProcessServerCommands(int operationTimeout, const char* saveDirectory)
	{
		OFCondition cond = EC_Normal;
		T_DIMSE_Message msg;
		T_ASC_PresentationContextID presID = 0;
		DcmDataset *statusDetail = NULL;

		// start a loop to be able to receive more than one DIMSE command
		while (cond == EC_Normal || cond == DIMSE_NODATAAVAILABLE)
		{
			// receive a DIMSE command over the network
			if (operationTimeout == -1)
		  		cond = DIMSE_receiveCommand(self, DIMSE_BLOCKING, 0, &presID, &msg, &statusDetail);
			else
			  	cond = DIMSE_receiveCommand(self, DIMSE_NONBLOCKING, operationTimeout, &presID, &msg, &statusDetail);

			// check what kind of error occurred. If no data was
			// received, check if certain other conditions are met
			if (cond == DIMSE_NODATAAVAILABLE)
			{
				// If in addition to the fact that no data was received also option --eostudy-timeout is set and
				// if at the same time there is still a study which is considered to be open (i.e. we were actually
				// expecting to receive more objects that belong to this study) (this is the case if lastStudyInstanceUID
				// does not equal NULL), we have to consider that all objects for the current study have been received.
				// In such an "end-of-study" case, we might have to execute certain optional functions which were specified
				// by the user through command line options passed to storescp.
				if( operationTimeout != -1)
				{
				}
			}

			// if the command which was received has extra status
			// detail information, dump this information
			if (statusDetail != NULL)
			{
				// printf("Extra Status Detail: \n");
				// statusDetail->print(COUT);
				delete statusDetail;
			}

			// check if peer did release or abort, or if we have a valid message
			if (cond == EC_Normal)
			{
				// in case we received a valid message, process this command
				// note that storescp can only process a C-ECHO-RQ and a C-STORE-RQ
				switch (msg.CommandField)
				{
					case DIMSE_C_ECHO_RQ:
						// process C-ECHO-Request
						cond = EchoScp(self, &msg, presID);
						break;
					case DIMSE_C_STORE_RQ:
						// process C-STORE-Request
						cond = StoreScp(self, &msg, presID, saveDirectory);
						break;
					case DIMSE_C_FIND_RQ:
						cond = FindScp(self, &msg.msg.CFindRQ, presID);
						break;
					case DIMSE_C_MOVE_RQ:
						cond = MoveScp(self, &msg.msg.CMoveRQ, presID);
						break;
					default:
						// we cannot handle this kind of message
						cond = DIMSE_BADCOMMANDTYPE;
						// fprintf(stderr, "storescp: Cannot handle command: 0x%x\n", OFstatic_cast(unsigned, msg.CommandField));
						string msg = string("Association::ProcessCommands: ") + cond.text();
						throw dicom_runtime_error(cond, msg);
				}
			}
		}
		return cond;
	}

	bool Release() throw (dicom_runtime_error)
	{
		OFCondition cond = EC_Normal;
		cond = ASC_releaseAssociation(self);

		if (cond != EC_Normal && cond.bad())
		{
			string msg = string("Release: ") + cond.text();
			throw dicom_runtime_error(cond, msg);
		}

		return true;
	}

	~T_ASC_Association() 
	{
		ASC_destroyAssociation(&self);
	}
}

/////////////////////////////////////////////////////////////////////////
//
// Specific references to types for SWIG to wrap
//
enum T_ASC_RejectParametersResult
{ 
    ASC_RESULT_REJECTEDPERMANENT			= 1, 
    ASC_RESULT_REJECTEDTRANSIENT			= 2
};

enum T_ASC_RejectParametersSource
{
    ASC_SOURCE_SERVICEUSER                          = 1,
    ASC_SOURCE_SERVICEPROVIDER_ACSE_RELATED         = 2,
    ASC_SOURCE_SERVICEPROVIDER_PRESENTATION_RELATED = 3
};

enum T_ASC_RejectParametersReason 
{ /* the following values are coded by DUL */
    /* Service User reasons */
    ASC_REASON_SU_NOREASON                          = 0x0101,
    ASC_REASON_SU_APPCONTEXTNAMENOTSUPPORTED        = 0x0102,
    ASC_REASON_SU_CALLINGAETITLENOTRECOGNIZED       = 0x0103,
    ASC_REASON_SU_CALLEDAETITLENOTRECOGNIZED        = 0x0107,
    /* Service Provider ACSE Related reasons */
    ASC_REASON_SP_ACSE_NOREASON                     = 0x0201,
    ASC_REASON_SP_ACSE_PROTOCOLVERSIONNOTSUPPORTED  = 0x0202,
    /* Service Provider Presentation Related reasons */
    ASC_REASON_SP_PRES_TEMPORARYCONGESTION          = 0x0301,
    ASC_REASON_SP_PRES_LOCALLIMITEXCEEDED           = 0x0302
};

struct T_ASC_RejectParameters
{
    T_ASC_RejectParametersResult result;
    T_ASC_RejectParametersSource source;
    T_ASC_RejectParametersReason reason;
};

enum T_ASC_NetworkRole
{
    NET_ACCEPTOR,		/* Provider Only */
    NET_REQUESTOR,		/* User Only */
    NET_ACCEPTORREQUESTOR	/* User and Provider */
};

////////////////////////////////////////////////////////////////////
//
// Specific references to functions for SWIG to wrap
// that are Association-oriented
//
OFCondition 
ASC_setAPTitles(
	T_ASC_Parameters * params,
	const char* callingAPTitle,
	const char* calledAPTitle,
	const char* respondingAPTitle);

OFCondition 
ASC_receiveAssociation(
	T_ASC_Network * network,
	T_ASC_Association ** association,
	long maxReceivePDUSize,
	void **associatePDU=NULL,
	unsigned long *associatePDUlength=NULL,
	OFBool useSecureLayer=OFFalse,
	DUL_BLOCKOPTIONS block=DUL_BLOCK,
	int timeout=0);

OFCondition 
ASC_getAPTitles(
	T_ASC_Parameters * params,
	char* callingAPTitle,
	char* calledAPTitle,
	char* respondingAPTitle);
void 
ASC_dumpParameters(T_ASC_Parameters * params, ostream& outstream);

OFCondition
ASC_rejectAssociation(
	T_ASC_Association * association,
	T_ASC_RejectParameters * rejectParameters,
	void **associatePDU=NULL,
	unsigned long *associatePDUlength=NULL);

OFCondition 
ASC_getApplicationContextName(
	T_ASC_Parameters * params,
	char* applicationContextName);

OFCondition
ASC_acknowledgeAssociation(
	T_ASC_Association * assoc,
	void **associatePDU=NULL,
	unsigned long *associatePDUlength=NULL);
int 
ASC_countAcceptedPresentationContexts(
	T_ASC_Parameters * params);

OFCondition 
ASC_acknowledgeRelease(T_ASC_Association * association);

OFCondition 
ASC_abortAssociation(T_ASC_Association * association);

OFCondition 
ASC_dropSCPAssociation(T_ASC_Association * association);

OFCondition 
ASC_destroyAssociation(T_ASC_Association ** association);

////////////////////////////////////////////////////////////////////
//
// Specific references to functions for SWIG to wrap
// that are DIMSE-oriented
//
void DIMSE_printCEchoRQ(FILE * f, T_DIMSE_C_EchoRQ * req);

OFCondition
DIMSE_sendEchoResponse(T_ASC_Association * assoc, 
	T_ASC_PresentationContextID presID,
	T_DIMSE_C_EchoRQ *request, DIC_US status, DcmDataset *statusDetail);

OFCondition
DIMSE_receiveCommand(T_ASC_Association *association,
			 T_DIMSE_BlockingMode blocking,
			 int timeout,
			 T_ASC_PresentationContextID *presID,
			 T_DIMSE_Message *msg, 
			 DcmDataset **statusDetail,
			 DcmDataset **commandSet=NULL);

struct T_DIMSE_C_FindRQ {
	DIC_US          MessageID;				/* M */
	DIC_UI          AffectedSOPClassUID;			/* M */
	T_DIMSE_Priority Priority;				/* M */
	T_DIMSE_DataSetType DataSetType;			/* M */
	/* Identifier provided as argument to DIMSE functions *//* M */
};

struct T_DIMSE_C_FindRSP {
	DIC_US          MessageIDBeingRespondedTo;		/* M */
	DIC_UI          AffectedSOPClassUID;			/* U(=) */
	T_DIMSE_DataSetType DataSetType;			/* M */
	DIC_US          DimseStatus;				/* M */
	/* Identifier provided as argument to DIMSE functions *//* C */
	unsigned int	opts; /* which optional items are set */
#define O_FIND_AFFECTEDSOPCLASSUID		0x0001
};

