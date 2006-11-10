
%include "dcmtk/ofstd/oftypes.h"
%include "dcmtk/ofstd/ofglobal.h"
%include "dcmtk/ofstd/ofcond.h"

//
// note that OFBool maps to bool
//
%template(OFGlobalBool) OFGlobal<OFBool>;
%template(OFGlobalUint32) OFGlobal<Uint32>;
