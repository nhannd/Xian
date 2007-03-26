%inline
%{
//-------------------------------------------
// Structs for interop; these need to be here
// so that C# code can access them
//-------------------------------------------
struct InteropStoreScpCallbackInfo
{
	char* FileName;
	char* CallingAETitle;
	DcmDataset* ImageDataset;
    T_DIMSE_StoreProgress *Progress;
    T_DIMSE_C_StoreRQ *Request;
	DcmDataset *StatusDetail;    
    DIC_US DimseStatus;
};

struct InteropStoreScuProgressInfo
{
	T_ASC_Association* Association;
	const char* CurrentFile;
	int TotalCount;
	int CurrentCount;
};

struct InteropStoreScuCallbackInfo
{
	char* CalledAETitle;
	const char* CurrentFile;
    T_DIMSE_StoreProgress * Progress;
    T_DIMSE_C_StoreRQ * Request;
	int CurrentCount;
	int TotalCount;
};

struct InteropFindScpCallbackInfo
{
	char* CallingAETitle;
	char* CallingPresentationAddress;
	
	T_DIMSE_C_FindRQ *Request;
	DcmDataset *RequestIdentifiers; 
	// out 
	T_DIMSE_C_FindRSP *Response;
	DcmDataset *ResponseIdentifiers;
	DcmDataset *StatusDetail;
};

struct InteropQueryRetrieveCallbackInfo 
{
    T_ASC_Association *assoc;
    T_ASC_PresentationContextID presId;
};

struct InteropMoveScpCallbackInfo
{
	char* CallingAETitle;
	char* CallingPresentationAddress;
	
	OFBool Cancelled; 
	T_DIMSE_C_MoveRQ *Request;
	DcmDataset *RequestIdentifiers; 
	// out 
	T_DIMSE_C_MoveRSP *Response;
	DcmDataset *ResponseIdentifiers;
	DcmDataset *StatusDetail;
};

struct InteropRetrieveCallbackInfo
{
	T_DIMSE_C_MoveRSP* CMoveResponse;
};
%}

%{
//
// ------------------------------------------------------------------------------------
// Define a series of types that are essentially data objects that hold a function 
// pointer. The function pointer in actually is a delegate in the C# managed world.
// DEPENDS: These typedefs are dependent on defined structs above.
// ------------------------------------------------------------------------------------
//
typedef void (SWIGSTDCALL* StoreScuCallbackHelperCallback)(InteropStoreScuCallbackInfo*);
static StoreScuCallbackHelperCallback CSharpStoreScuCallbackHelperCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterStoreScuCallbackHelper_OffisDcm(StoreScuCallbackHelperCallback callback) {
	CSharpStoreScuCallbackHelperCallback = callback;
}
//-----------------------------------
typedef void (SWIGSTDCALL* RetrieveCallbackHelperCallback)(InteropRetrieveCallbackInfo*);
static RetrieveCallbackHelperCallback CSharpRetrieveCallbackHelperCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterRetrieveCallbackHelper_OffisDcm(RetrieveCallbackHelperCallback callback) {
	CSharpRetrieveCallbackHelperCallback = callback;
}
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
typedef void (SWIGSTDCALL* MoveScpCallbackHelper_MoveBeginCallback)(InteropMoveScpCallbackInfo*);
static MoveScpCallbackHelper_MoveBeginCallback CSharpMoveScpCallbackHelper_MoveBeginCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterMoveScpCallbackHelper_MoveBegin_OffisDcm(MoveScpCallbackHelper_MoveBeginCallback callback) {
	CSharpMoveScpCallbackHelper_MoveBeginCallback = callback;
}
//--------------------------------------------------------------
typedef void (SWIGSTDCALL* MoveScpCallbackHelper_MoveNextResponseCallback)(InteropMoveScpCallbackInfo*);
static MoveScpCallbackHelper_MoveNextResponseCallback CSharpMoveScpCallbackHelper_MoveNextResponseCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm(MoveScpCallbackHelper_MoveNextResponseCallback callback) {
	CSharpMoveScpCallbackHelper_MoveNextResponseCallback = callback;
}
//--------------------------------------------------------------
typedef void (SWIGSTDCALL* FindScpCallbackHelper_QueryDBCallback)(InteropFindScpCallbackInfo*);
static FindScpCallbackHelper_QueryDBCallback CSharpFindScpCallbackHelper_QueryDBCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterFindScpCallbackHelper_QueryDB_OffisDcm(FindScpCallbackHelper_QueryDBCallback callback) {
	CSharpFindScpCallbackHelper_QueryDBCallback = callback;
}
//--------------------------------------------------------------
typedef void (SWIGSTDCALL* FindScpCallbackHelper_GetNextFindResponseCallback)(InteropFindScpCallbackInfo*);
static FindScpCallbackHelper_GetNextFindResponseCallback CSharpFindScpCallbackHelper_GetNextFindResponseCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm(FindScpCallbackHelper_GetNextFindResponseCallback callback) {
	CSharpFindScpCallbackHelper_GetNextFindResponseCallback = callback;
}
//--------------------------------------------------------------
typedef void (SWIGSTDCALL* StoreScpCallbackHelper_StoreBeginCallback)(InteropStoreScpCallbackInfo*);
static StoreScpCallbackHelper_StoreBeginCallback CSharpStoreScpCallbackHelper_StoreBeginCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm(StoreScpCallbackHelper_StoreBeginCallback callback) {
	CSharpStoreScpCallbackHelper_StoreBeginCallback = callback;
}
//--------------------------------------------------------------
typedef void (SWIGSTDCALL* StoreScpCallbackHelper_StoreProgressCallback)(InteropStoreScpCallbackInfo*);
static StoreScpCallbackHelper_StoreProgressCallback CSharpStoreScpCallbackHelper_StoreProgressCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterStoreScpCallbackHelper_StoreProgress_OffisDcm(StoreScpCallbackHelper_StoreProgressCallback callback) {
	CSharpStoreScpCallbackHelper_StoreProgressCallback = callback;
}
//--------------------------------------------------------------
typedef void (SWIGSTDCALL* StoreScpCallbackHelper_StoreEndCallback)(InteropStoreScpCallbackInfo*);
static StoreScpCallbackHelper_StoreEndCallback CSharpStoreScpCallbackHelper_StoreEndCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(StoreScpCallbackHelper_StoreEndCallback callback) {
	CSharpStoreScpCallbackHelper_StoreEndCallback = callback;
}
//--------------------------------------------------------------
%}
