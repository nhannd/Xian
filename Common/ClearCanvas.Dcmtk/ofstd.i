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
// Leave out undefined constructors and operators in OFGlobal.h
#define SWIG

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

#define base baseCond

%include "osconfig.h"
%include "oftypes.h"
%include "ofglobal.h"
%include "oflist.h"
%include "ofstring.h"
%include "ofcond.h"
%include "ofdate.h"
%include "ofdatime.h"
%include "oftime.h"
