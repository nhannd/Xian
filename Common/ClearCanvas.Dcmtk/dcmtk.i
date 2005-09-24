///////////////////////////////////////////////////////////////////////////////
//
// Main SWIG Interface file.
//
// $Log: dcmtk.i,v $
// Revision 1.3  2004/11/15 21:04:20  Norman
// Initial revision
//
//
///////////////////////////////////////////////////////////////////////////////

%module DCMTK

%{
#pragma warning (disable:4267)
#pragma warning (disable:4800)
%}

%include "typemaps.i"
%include "dcm_typemaps.i"
%include "std_string.i"

%apply unsigned char *OUTPUT {Uint8 &};
%apply unsigned short *OUTPUT {Uint16 &};
%apply short *OUTPUT {Sint16 &};
%apply unsigned int *OUTPUT {Uint32 &};
%apply int *OUTPUT {Sint32 &};
%apply int *OUTPUT {long int &};
%apply float *OUTPUT {Float32 &};
%apply double *OUTPUT {Float64 &};
%apply double *OUTPUT {double &};
%apply std::string & {std::string &};

%apply const unsigned char *INPUT_ARRAY {Uint8 *};
%apply const unsigned short *INPUT_ARRAY {Uint16 *};
%apply const short *INPUT_ARRAY {Sint16 *};
%apply const unsigned int *INPUT_ARRAY {Uint32 *};
%apply const int *INPUT_ARRAY {Sint32 *};
%apply const float *INPUT_ARRAY {Float32 *};
%apply const double *INPUT_ARRAY {Float64 *};

%apply char *&OUTPUT_ARRAY {char *&};
%apply unsigned char *&OUTPUT_ARRAY {Uint8 *&};
%apply unsigned short *&OUTPUT_ARRAY {Uint16 *&};
%apply short *&OUTPUT_ARRAY {Sint16 *&};
%apply unsigned int *&OUTPUT_ARRAY {Uint32 *&};
%apply int *&OUTPUT_ARRAY {Sint32 *&};
%apply float *&OUTPUT_ARRAY {Float32 *&};
%apply double *&OUTPUT_ARRAY {Float64 *&};

%include "ofstd.i"
%include "dcmdata.i"
%include "dcmnet.i"

