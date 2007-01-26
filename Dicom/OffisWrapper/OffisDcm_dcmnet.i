%module OffisDcm

//
// HACK: For some reason SWIG can't detect
// that multiple defines if they define the
// same value, does not introduce ambiguity and
// balks when compiling T_DIMSE_N_CreateRQ and
// T_DIMSE_N_CreateRSP
//
%ignore T_DIMSE_N_CreateRQ;

//
// Fundamental typemaps and declarations
//
%include "std_vector.i"
%include "std_string.i"
%include "typemaps.i"
%include "OffisDcm_typemaps.i"
%include "OffisDcm_ofstd.i"

CONTROLACCESSPUBLIC_DERIVED(DcmDataset)
CONTROLACCESSPUBLIC(T_DIMSE_C_FindRSP)
CONTROLACCESSPUBLIC(T_DIMSE_C_FindRQ)
CONTROLACCESSPUBLIC(T_DIMSE_C_MoveRSP)
CONTROLACCESSPUBLIC(InteropStoreScpCallbackInfo)
CONTROLACCESSPUBLIC(InteropFindScpCallbackInfo)
CONTROLACCESSPUBLIC(InteropMoveScpCallbackInfo)
CONTROLACCESSPUBLIC(InteropStoreScuCallbackInfo)
CONTROLACCESSPUBLIC(InteropRetrieveCallbackInfo)

%{
#include <string>
#include <vector>
#include "dcmtk/config/osconfig.h"
#include "dcmtk/dcmnet/assoc.h"
#include "dcmtk/ofstd/ofcond.h"
#include "dcmtk/dcmnet/cond.h"
#include "dcmtk/dcmnet/diutil.h"
#include "dcmtk/dcmnet/dimse.h"
#include "dcmtk/dcmdata/dcdatset.h"
%}

//
// Typemaps and other definitions that are necessary
// for defining other mappings 
//
%include "dcmtk/dcmnet/dicom.h"
%include "dcmtk/dcmnet/cond.h"
%include "dcmtk/dcmnet/diutil.h"
%include "dcmtk/dcmnet/dimse.h"
%include "OffisDcm_insertions.i"
%include "OffisDcm_dcmdata.i"
%include "OffisDcm_dcmnet_interop.i"
%include "OffisDcm_dcmnet_utilities.i"
%include "OffisDcm_dcmnet_interop_functions.i"
%include "OffisDcm_dcmnet_utilities_functions.i"
%include "OffisDcm_dcmnet_network.i"
%include "OffisDcm_dcmnet_association.i"
%include "OffisDcm_dcmnet_parameters.i"

%inline
%{

//-------------------------------------------
// Helper function to set the connection
// timeout in the OFFIS tk
//-------------------------------------------
void SetGlobalConnectionTimeout(int newTimeout)
{
	dcmConnectionTimeout.set((Sint32) newTimeout);
}

void SetReverseDnsLookupFlag(bool enable)
{
	dcmDisableGethostbyaddr.set((OFBool) enable);
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

%}


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

