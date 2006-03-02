##################################################################
##
## Declaration of variables
## 
##################################################################
Configuration = Debug
dcmtkBuildDir = OFFIS/build/$(Configuration)
SwigOutputCsharpDir = csharp
CppCompilerOutputDirRoot = cppwrapper
CppCompilerOutputDir = $(CppCompilerOutputDirRoot)/$(Configuration)
SwigOutputNamespace = ClearCanvas.Dicom.OffisWrapper
CppCompilerParameters = -DSWIG -DWITH_LIBTIFF -DHAVE_CXX_BOOL -DHAVE_STD_STRING -DHAVE_CONFIG_H
CppCompilerIncludeParameters = -I../../$(dcmtkBuildDir)/include \
	-I../../$(dcmtkBuildDir)/include/ofstd \
	-I../../$(dcmtkBuildDir)/include/dcmdata \
	-I../../$(dcmtkBuildDir)/include/dcmnet
CppLibraries = -ldcmnet -ldcmdata -lofstd -lz -lstdc++ -lpthread
CppLibraryPath = -L../../$(dcmtkBuildDir)/lib
ifeq ($(Configuration), Debug)
	CppCompilerDebugParameters = -O0 -g -DDEBUG
	CscEmitDebugInformation = +
else
	CppCompilerDebugParameters = -O2
	CscEmitDebugInformation = -
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
	swig -csharp -c++ -DHAVE_CXX_BOOL -DHAVE_STD_STRING -DHAVE_CONFIG_H $(CppCompilerIncludeParameters) \
	-outdir $(SwigOutputCsharpDir) -Wall -makedefault -o $(SwigCppOutputFile) \
	-namespace $(SwigOutputNamespace) $(SwigInterfaceFile)

libOffisDcm.so : Offis_Wrapper.cxx 
	rm -r -f $(CppCompilerOutputDir)
	mkdir -p $(CppCompilerOutputDir)
	gcc -fpic -c $(CppCompilerParameters) $(CppCompilerDebugParameters) $(CppCompilerIncludeParameters) $(SwigCppOutputFile) -o $(CppCompilerOutputDir)/offisdcm.o
	gcc -z defs -shared -o $(CppCompilerOutputDir)/libOffisDcm.so $(CppCompilerOutputDir)/offisdcm.o $(CppLibraryPath) $(CppLibraries)

$(SwigOutputNamespace).dll : Offis_Wrapper.cxx
	mkdir -p $(SwigOutputCsharpDir)/$(Configuration)
	mcs -out:$(SwigOutputCsharpDir)/$(Configuration)/$(SwigOutputNamespace).dll \
	-target:library -debug$(CscEmitDebugInformation) \
	$(SwigOutputCsharpDir)/*.cs

all: $(SwigOutputNamespace).dll libOffisDcm.so ;

clean:
	rm -f Offis_Wrapper.cxx
	rm -r -f $(SwigOutputCsharpDir)
	rm -r -f $(CppCompilerOutputDir)
