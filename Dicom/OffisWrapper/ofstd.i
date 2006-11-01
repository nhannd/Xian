///////////////////////////////////////////////////////////////////////////////
//
// SWIG Interface file for ofstd library
//
// $Log: ofstd.i,v $
// Revision 1.1  2004/11/15 21:02:01  Norman
// Initial version
//
//
///////////////////////////////////////////////////////////////////////////////

%{
#include "dcmtk/config/osconfig.h"
#include "dcmtk/ofstd/ofglobal.h"
#include "dcmtk/ofstd/oflist.h"
#include "dcmtk/ofstd/ofstring.h"
#include "dcmtk/ofstd/oftypes.h"
#include "dcmtk/ofstd/ofcond.h"
#include "dcmtk/ofstd/ofdate.h"
#include "dcmtk/ofstd/ofdatime.h"
#include "dcmtk/ofstd/oftime.h"
%}

#ifndef base
	#define base baseCond
#endif

%include "dcmtk/config/osconfig.h"
%include "dcmtk/ofstd/ofglobal.h"
%include "dcmtk/ofstd/oflist.h"
%include "dcmtk/ofstd/ofstring.h"
%include "dcmtk/ofstd/oftypes.h"
%include "dcmtk/ofstd/ofcond.h"
%include "dcmtk/ofstd/ofdate.h"
%include "dcmtk/ofstd/ofdatime.h"
%include "dcmtk/ofstd/oftime.h"
