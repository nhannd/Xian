%{
// ---------------------------------------------
// structs used for internal utilities function
// ---------------------------------------------
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

%}
