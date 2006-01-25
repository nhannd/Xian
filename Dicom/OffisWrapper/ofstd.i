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
#include "osconfig.h"
#include "ofglobal.h"
#include "oflist.h"
#include "ofstring.h"
#include "oftypes.h"
#include "ofcond.h"
#include "ofdate.h"
#include "ofdatime.h"
#include "oftime.h"
%}

#ifndef base
	#define base baseCond
#endif

%include "osconfig.h"
%include "oftypes.h"
%include "ofglobal.h"
%include "oflist.h"
%include "ofstring.h"
%include "ofcond.h"
%include "ofdate.h"
%include "ofdatime.h"
%include "oftime.h"
