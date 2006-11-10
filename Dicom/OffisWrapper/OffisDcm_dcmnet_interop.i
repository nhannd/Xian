%inline
%{
//-------------------------------------------
// Structs for interop; these need to be here
// so that C# code can access them
//-------------------------------------------
struct InteropStoreCallbackInfo
{
	const char* FileName;
	DcmDataset* ImageDataset;
};

struct InteropStoreScuFileCountProgressInfo
{
	int TotalCount;
	int CurrentCount;
};

struct InteropStoreScuCallbackInfo
{
    T_DIMSE_StoreProgress * Progress;
    T_DIMSE_C_StoreRQ * Request;
	int CurrentCount;
	int TotalCount;
};

struct InteropFindScpCallbackInfo
{
	OFBool Cancelled; 
	T_DIMSE_C_FindRQ *Request;
	DcmDataset *RequestIdentifiers; 
	int ResponseCount;
	// out 
	T_DIMSE_C_FindRSP *Response;
	DcmDataset *ResponseIdentifiers;
	DcmDataset *StatusDetail;
};

struct InteropQueryRetrieveCallbackInfo {
    T_ASC_Association *assoc;
    T_ASC_PresentationContextID presId;
};

struct InteropMoveCallbackInfo
{

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
// -----------------------------------
typedef void (SWIGSTDCALL* StoreCallbackHelperCallback)(InteropStoreCallbackInfo*);
static StoreCallbackHelperCallback CSharpStoreCallbackHelperCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterStoreCallbackHelper_OffisDcm(StoreCallbackHelperCallback callback) {
	CSharpStoreCallbackHelperCallback = callback;
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
typedef void (SWIGSTDCALL* MoveCallbackHelperCallback)(InteropMoveCallbackInfo*);
static MoveCallbackHelperCallback CSharpMoveCallbackHelperCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterMoveCallbackHelper_OffisDcm(MoveCallbackHelperCallback callback) {
	CSharpMoveCallbackHelperCallback = callback;
}
//--------------------------------------------------------------
typedef void (SWIGSTDCALL* FindScpCallbackHelperCallback)(InteropFindScpCallbackInfo*);
static FindScpCallbackHelperCallback CSharpFindScpCallbackHelperCallback = NULL;

#ifdef __cplusplus
extern "C" 
#endif
SWIGEXPORT void SWIGSTDCALL RegisterFindScpCallbackHelper_OffisDcm(FindScpCallbackHelperCallback callback) {
	CSharpFindScpCallbackHelperCallback = callback;
}
//--------------------------------------------------------------
%}
