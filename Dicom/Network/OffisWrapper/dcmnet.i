%module dcmnet

%{
#include "assoc.h"
#include "dimse.h"
#include "ofcond.h"
%}

#define params parameters
#define base baseclass

%include "ofcond.h"
%include "dicom.h"

/////////////////////////////////////////////////////////////////////////
//
// SECTION: Custom Exception
// The following section implements the support for a custom exception
// to be thrown from the C++ code and caught in the C# code
//
/////////////////////////////////////////////////////////////////////////
%insert(runtime) %{

// Code to handle throwing of C# DicomRuntimeApplicationException from C/C++ code.
// The equivalent delegate to the callback, CSharpExceptionCallback_t, is DicomRuntimeExceptionDelegate
// and the equivalent dicomRuntimeExceptionCallback instance is dicomRuntimeDelegate
typedef void (SWIGSTDCALL* CSharpExceptionCallback_t)(const char *);
CSharpExceptionCallback_t dicomRuntimeExceptionCallback = NULL;

extern "C" SWIGEXPORT
void SWIGSTDCALL DicomRuntimeExceptionRegisterCallback(CSharpExceptionCallback_t dicomRuntimeCallback) {
	dicomRuntimeExceptionCallback = dicomRuntimeCallback;
}

// Note that SWIG detects any method calls named starting with
// SWIG_CSharpSetPendingException for warning 845
static void SWIG_CSharpSetPendingExceptionDicomRuntime(const char *msg) {
	dicomRuntimeExceptionCallback(msg);
}

// custom define to change "parameters" variable names back to params in C++ world
#define parameters params
%}

%pragma(csharp) imclassimports=%{
using System;
using System.Runtime.InteropServices;

// Custom C# Exception
public class DicomRuntimeApplicationException : System.ApplicationException {
  public DicomRuntimeApplicationException(string message) 
    : base(message) {
  }
}
%}

%pragma(csharp) imclasscode=%{

	class DicomRuntimeExceptionHelper 
	{
		// C# delegate for the C/C++ dicomRuntimeExceptionCallback
		public delegate void DicomRuntimeExceptionDelegate(string message);
		static DicomRuntimeExceptionDelegate dicomRuntimeDelegate = new DicomRuntimeExceptionDelegate(SetPendingDicomRuntimeException);

		[DllImport("$dllimport", EntryPoint="DicomRuntimeExceptionRegisterCallback")]
		public static extern
		void DicomRuntimeExceptionRegisterCallback(DicomRuntimeExceptionDelegate dicomRuntimeCallback);

		static void SetPendingDicomRuntimeException(string message) 
		{
			SWIGPendingException.Set(new DicomRuntimeApplicationException(message));
		}

		static DicomRuntimeExceptionHelper() 
		{
			DicomRuntimeExceptionRegisterCallback(dicomRuntimeDelegate);
		}
	}
	static DicomRuntimeExceptionHelper exceptionHelper = new DicomRuntimeExceptionHelper();

%}

%typemap(throws, canthrow=1) std::runtime_error {
	  SWIG_CSharpSetPendingExceptionDicomRuntime($1.what());
	  return $null;
}
/////////////////////////////////////////////////////////////////////////
//
// END OF SECTION: Custom Exception
//
/////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////
//
// SECTION: Extension of the T_ASC_Network class
//
/////////////////////////////////////////////////////////////////////////
struct T_ASC_Network
{
    T_ASC_NetworkRole   role;
    int             	acceptorPort;
    DUL_NETWORKKEY      *network;
};

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

%extend(canthrow=1) T_ASC_Network {
	T_ASC_Network(T_ASC_NetworkRole role,
					int acceptorPort,
					int timeout) throw (std::runtime_error)
	{
		T_ASC_Network* pNetwork = 0;
		OFCondition result = ASC_initializeNetwork(role, 
			acceptorPort, 
			timeout, 
			&pNetwork);

		if (result.bad())
		{
			string msg = string("T_ASC_Network ctor: ") + result.text();
			throw std::runtime_error(msg);
		}
		
		return pNetwork;
	}

	T_ASC_Association* CreateAssociation(T_ASC_Parameters* associationParameters)
		throw (std::runtime_error)
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
				
				throw std::runtime_error(msg);
			} 
			else 
			{
				string msg = string("Association request failed: ") + result.text();
				throw std::runtime_error(msg);
			}
			
		}

		if (0 == ASC_countAcceptedPresentationContexts(associationParameters)) 
		{
			// clean up the allocated association before throwing exception
			ASC_destroyAssociation(&pAssociation);
			throw std::runtime_error("T_ASC_Network.CreateAssociation: No acceptable Presentation Contexts");
		}

		return pAssociation;
	}

	~T_ASC_Network() 
	{
		ASC_dropNetwork(&self);
	}
}
/////////////////////////////////////////////////////////////////////////
//
// END OF SECTION: Extension of the T_ASC_Network class
//
/////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////
//
// SECTION: Extension of the T_ASC_Parameters class
//
/////////////////////////////////////////////////////////////////////////
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
	T_ASC_Parameters(int maxReceivePDULength,
	const char* ourAETitle,
	const char* peerAETitle,
	const char* peerHostname,
	int peerPort) throw (std::runtime_error)
	{
		T_ASC_Parameters* pParameters = 0;
		OFCondition result = ASC_createAssociationParameters(&pParameters,
			maxReceivePDULength);

		if (result.bad())
		{
			string msg = string("ASC_createAssociationParameters: ") + result.text();
			throw std::runtime_error(msg);
		}

		result = ASC_setAPTitles(pParameters, ourAETitle, peerAETitle, NULL);

		if (result.bad())
		{
			string msg = string("ASC_setAPTitles: ") + result.text();
			throw std::runtime_error(msg);
		}
		
		// we will use an unsecured transport layer at this point (False)
		result = ASC_setTransportLayerType(pParameters, OFFalse);

		if (result.bad())
		{
			string msg = string("ASC_setTransportLayerType: ") + result.text();
			throw std::runtime_error(msg);
		}

		DIC_NODENAME localHost;
		DIC_NODENAME peerHost;

		gethostname(localHost, sizeof(localHost) - 1);
		sprintf(peerHost, "%s:%d", peerHostname, (int) peerPort);
		result = ASC_setPresentationAddresses(pParameters, localHost, peerHost);

		if (result.bad())
		{
			string msg = string("ASC_setPresentationAddresses: ") + result.text();
			throw std::runtime_error(msg);
		}

		static const char* transferSyntaxes[] = {
			UID_LittleEndianImplicitTransferSyntax, /* default xfer syntax first */
			UID_LittleEndianExplicitTransferSyntax,
			UID_BigEndianExplicitTransferSyntax,
			UID_JPEGProcess1TransferSyntax,
			UID_JPEGProcess2_4TransferSyntax,
			UID_JPEGProcess3_5TransferSyntax,
			UID_JPEGProcess6_8TransferSyntax,
			UID_JPEGProcess7_9TransferSyntax,
			UID_JPEGProcess10_12TransferSyntax,
			UID_JPEGProcess11_13TransferSyntax,
			UID_JPEGProcess14TransferSyntax,
			UID_JPEGProcess15TransferSyntax,
			UID_JPEGProcess16_18TransferSyntax,
			UID_JPEGProcess17_19TransferSyntax,
			UID_JPEGProcess20_22TransferSyntax,
			UID_JPEGProcess21_23TransferSyntax,
			UID_JPEGProcess24_26TransferSyntax,
			UID_JPEGProcess25_27TransferSyntax,
			UID_JPEGProcess28TransferSyntax,
			UID_JPEGProcess29TransferSyntax,
			UID_JPEGProcess14SV1TransferSyntax,
			UID_RLELosslessTransferSyntax,
			UID_JPEGLSLosslessTransferSyntax,
			UID_JPEGLSLossyTransferSyntax,
			UID_DeflatedExplicitVRLittleEndianTransferSyntax,
			UID_JPEG2000LosslessOnlyTransferSyntax,
			UID_JPEG2000TransferSyntax,
			UID_MPEG2MainProfileAtMainLevelTransferSyntax
		};

		result = ASC_addPresentationContext(pParameters, 1, UID_VerificationSOPClass,
                 transferSyntaxes, 1);

		if (result.bad())
		{
			string msg = string("ASC_addPresentationContext: ") + result.text();
			throw std::runtime_error(msg);
		}

		return pParameters;
	}

	~T_ASC_Parameters() 
	{
		ASC_destroyAssociationParameters(&self);
	}
}

/////////////////////////////////////////////////////////////////////////
//
// END OF SECTION: Extension of the T_ASC_Parameters class
//
/////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////
//
// SECTION: Extension of the T_ASC_Association class
//
/////////////////////////////////////////////////////////////////////////
struct T_ASC_Association
{
    DUL_ASSOCIATIONKEY *DULassociation;
    T_ASC_Parameters *params;

    unsigned short nextMsgID;	        /* should be incremented by user */
    unsigned long sendPDVLength;	/* max length of PDV to send out */
    unsigned char *sendPDVBuffer;	/* buffer of size sendPDVLength */
};

%extend(canthrow=1) T_ASC_Association {

	bool SendCEcho(int numberOfCEchoRepeats) throw (std::runtime_error)
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
			throw std::runtime_error(msg);
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
// END OF SECTION: Extension of the T_ASC_Association class
//
/////////////////////////////////////////////////////////////////////////

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


//
// Association functions
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

//
// DIMSE functions
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

//
// Types that need to be declared
// 
// T_DIMSE_Message;
// T_ASC_PresentationContextID;
// DcmDataset;
// OFString;
// T_ASC_Network;
// T_ASC_Association;
// OFCondition;
// T_ASC_RejectParameters;
// ASC_acceptContextsWithPreferredTransferSyntaxes();

//
// Others
//
// DIMSE_NODATAAVAILABLE;
// DIMSE_BLOCKING;
// DIMSE_NONBLOCKING;
// NET_ACCEPTOR;
// UID_VerificationSOPClass;
// ASC_RESULT_REJECTEDPERMANENT;
// ASC_SOURCE_SERVICEUSER;
// ASC_REASON_SU_NOREASON;
