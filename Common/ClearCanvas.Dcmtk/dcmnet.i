///////////////////////////////////////////////////////////////////////////////
//
// SWIG Interface file for dcmnet library
//
// $Log: dcmnet.i,v $
// Revision 1.2  2004/11/15 21:02:01  Norman
// Initial version
//
//
///////////////////////////////////////////////////////////////////////////////

%{
#include "assoc.h"
#include "cond.h"
#include "dcasccff.h"
#include "dcasccfg.h"
#include "dicom.h"
#include "dimse.h"
#include "diutil.h"
#include "lst.h"
%}


// "params" is a keyword in C#, so rename it to "parameters"
// This will generate two compiler errors.  They are fixed by editing the
// dcmtk_wrap.cxx file so that "parameters" is changed back to "params".
#define params parameters

// Ignore this for now, since the return value is causing problems
%ignore AttributeIdentifierList;

// Ignore this, since the #define above changed "T_ASC_Association::params" to 
// "T_ASC_Association::parameters".  We manually reimplement the wrapper for this 
// member below.

%ignore T_ASC_Association::parameters;

%include "dicom.h"
%include "assoc.h"
%include "cond.h"
%include "dcasccff.h"
%include "dcasccfg.h"
%include "dimse.h"
%include "diutil.h"
%include "lst.h"

%wrapper
%{

SWIGEXPORT void SWIGSTDCALL CSharp_set_T_ASC_Association_parameters(void * jarg1, void * jarg2) {
	T_ASC_Association *arg1 = (T_ASC_Association *) 0 ;
	T_ASC_Parameters *arg2 = (T_ASC_Parameters *) 0 ;
	
	arg1 = (T_ASC_Association *)jarg1; 
	arg2 = (T_ASC_Parameters *)jarg2; 
	if (arg1) (arg1)->params = arg2;
	
}


SWIGEXPORT void * SWIGSTDCALL CSharp_get_T_ASC_Association_parameters(void * jarg1) {
	void * jresult = 0 ;
	T_ASC_Association *arg1 = (T_ASC_Association *) 0 ;
	T_ASC_Parameters *result;
	
	arg1 = (T_ASC_Association *)jarg1; 
	result = (T_ASC_Parameters *) ((arg1)->params);
	
	jresult = (void *)result; 
	return jresult;
}

%}