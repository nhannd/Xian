##################################################################
#
# Declaration of variables
# 
##################################################################
Configuration = Debug
SwigOutputCsharpDir = csharp
CppCompilerOutputDirRoot = cppwrapper
CppCompilerOutputDir = $(CppCompilerOutputDirRoot)/$(Configuration)
SwigOutputNamespace = ClearCanvas.Dicom.OffisWrapper
CppCompilerParameters = -DSWIG -DWITH_LIBPNG -DWITH_LIBTIFF \
	-DWITH_ZLIB -DHAVE_STD_STRING -DHAVE_CXX_BOOL -DHAVE_CONFIG_H
CppCompilerIncludeParameters = -I../../OFFIS/dcmtk-3.5.3/config/include \
	-I../../OFFIS/dcmtk-3.5.3/ofstd/include \
	-I../../OFFIS/dcmtk-3.5.3/dcmdata/include \
	-I../../OFFIS/dcmtk-3.5.3/dcmnet/include \
	-I../../OFFIS/dcmtk-3.5.3/ofstd/include \
	-I../../OFFIS/dcmtk-3.5.3/dcmdata/include \
	-I../../OFFIS/dcmtk-3.5.3/zlib-1.2.1/include
CppLinkerObjectFiles = ../../OFFIS/bin/libofstd.a ../../OFFIS/bin/libdcmdata.a ../../OFFIS/bin/libdcmnet.a 
ifeq ($(Configuration), Debug)
	CppCompilerDebugParameters = -O0 -g
	CppLinkerParameters = -L../../OFFIS/bin/debug/lib \
	-L../../OFFIS/zlib-1.2.1/lib
	CppLinkerDebugParameters = -D DEBUG -lz 
	CppLinkerZlibFile = zlib_d.lib
	CscEmitDebugInformation = +
	CppLinkerObjectFiles = ../../OFFIS/bin/debug/lib/libofstd.a ../../OFFIS/bin/debug/lib/libdcmdata.a ../../OFFIS/bin/debug/lib/libdcmnet.a 
else
	CppCompilerDebugParameters = -O2
	CppLinkerParameters = -L../../OFFIS/bin/release/lib \
	-L../../OFFIS/zlib-1.2.1/lib
	CppLinkerDebugParameters = 
	CppLinkerZlibFile = zlib_o.lib
	CscEmitDebugInformation = -
	CppLinkerObjectFiles = ../../OFFIS/bin/libofstd.a ../../OFFIS/bin/libdcmdata.a ../../OFFIS/bin/libdcmnet.a 
	CppLinkerObjectFiles = ../../OFFIS/bin/release/lib/libofstd.a ../../OFFIS/bin/release/lib/libdcmdata.a ../../OFFIS/bin/release/lib/libdcmnet.a 
endif
SwigInterfaceFile = dcmdata.i
SwigCppOutputFile = Offis_Wrapper.cxx
VPATH = $(CppCompilerOutputDir):$(SwigOutputCSharpDir)\$(Configuration)

##################################################################
#
# Declaration of rules
# 
##################################################################

Offis_Wrapper.cxx : dcmdata.i dcmnet.i dcm_typemaps.i ofstd.i
	rm -r -f $(SwigOutputCsharpDir)
	mkdir $(SwigOutputCsharpDir)
	swig -csharp -c++ -DHAVE_STD_STRING -DHAVE_CXX_BOOL -DHAVE_CONFIG_H\
	-I../../OFFIS/dcmtk-3.5.3/config/include \
	-I../../OFFIS/dcmtk-3.5.3/ofstd/include \
	-I../../OFFIS/dcmtk-3.5.3/dcmdata/include \
	-I../../OFFIS/dcmtk-3.5.3/dcmnet/include \
	-outdir $(SwigOutputCsharpDir) -Wall -makedefault -o $(SwigCppOutputFile) \
	-namespace $(SwigOutputNamespace) $(SwigInterfaceFile)

liboffisdcm.so : Offis_Wrapper.cxx 
	rm -r -f $(CppCompilerOutputDir)
	mkdir -p $(CppCompilerOutputDir)
	gcc -fpic -c $(CppCompilerParameters) $(CppCompilerDebugParameters) $(CppCompilerIncludeParameters) $(SwigCppOutputFile) -o $(CppCompilerOutputDir)/offisdcm.o
	gcc -shared $(CppLinkerParameters) $(CppLinkerDebugParameters) $(CppLinkerObjectFiles) $(CppCompilerOutputDir)/offisdcm.o -o $(CppCompilerOutputDir)/liboffisdcm.so

$(SwigOutputNamespace).dll : Offis_Wrapper.cxx
	mkdir -p $(SwigOutputCsharpDir)/$(Configuration)
	mcs -out:$(SwigOutputCsharpDir)/$(Configuration)/$(SwigOutputNamespace).dll \
	-target:library -debug$(CscEmitDebugInformation) \
	$(SwigOutputCsharpDir)/*.cs

all: $(SwigOutputNamespace).dll liboffisdcm.so
