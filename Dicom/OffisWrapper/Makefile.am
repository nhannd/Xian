## Set project parameters
projectName=Dicom.OffisWrapper
projectNamespace = ClearCanvas.Dicom.OffisWrapper
dllFilename = $(projectNamespace).dll

## Set file and directory parameters
SWIG_INTERFACE = dcmdata.i
SWIG_OUTPUT_FILE = Offis_Wrapper.cxx
SWIG_OUTPUT_DIR = csharp
SWIG_DEPEND = dcmdata.i dcmnet.i dcm_typemaps.i ofstd.i
DCMTK_DIR = @SOURCE_PATH@/OFFIS/dcmtk-3.5.3
DCMTK_DEPEND = $(DCMTK_DIR)/config/Makefile
DCMTK_BUILD_DIR = @INSTALL_PATH@
DCMTK_INCLUDE = $(DCMTK_BUILD_DIR)/include
DCMTK_LIBS_DIR = $(DCMTK_BUILD_DIR)/lib
DCMTK_LIBS = $(DCMTK_LIBS_DIR)/libdcmnet.a $(DCMTK_LIBS_DIR)/libdcmdata.a $(DCMTK_LIBS_DIR)/libofstd.a
NATIVE_OUTPUT_DIR = cppwrapper/$(CONFIGURATION)
NATIVE_FILE = libOffisDcm.so
NATIVE_OBJECT = offisdcm.o
NATIVE_FILE_LIBS = $(DCMTK_LIBS) -lz -lstdc++ -lpthread

## Generated files that are removed on a make clean
CLEANFILES= \
	$(NATIVE_OUTPUT_DIR)/$(NATIVE_FILE)\
	$(NATIVE_OUTPUT_DIR)/$(NATIVE_OBJECT)\
	@BUILD_PATH@/$(NATIVE_FILE)\
	$(SWIG_OUTPUT_FILE)\
	$(SWIG_OUTPUT_DIR)/*.cs\
	@BUILD_PATH@/$(dllFilename)\
	$(SWIG_OUTPUT_DIR)/$(CONFIGURATION)/$(dllFilename)\
	$(DCMTK_BUILD_DIR)/bin\
	$(DCMTK_BUILD_DIR)/etc\
	$(DCMTK_BUILD_DIR)/lib\
	$(DCMTK_BUILD_DIR)/include\
	$(DCMTK_BUILD_DIR)/man\
	$(DCMTK_BUILD_DIR)/share

## Files included in a distribution
EXTRA_DIST = \
	$(SWIG_DEPEND) \
	CreateOffisWrappers.*

## Run the configure and make in the OFFIS package
$(DCMTK_LIBS) : $(DCMTK_DEPEND)
	$(install_sh) -d $(DCMTK_BUILD_DIR)
	sed -e 's/CXXFLAGS =/CXXFLAGS = -DHAVE_STD_STRING/' $(DCMTK_DEPEND).def > $(DCMTK_DEPEND).def2
	mv -f $(DCMTK_DEPEND).def2 $(DCMTK_DEPEND).def
	$(MAKE) -C $(DCMTK_DIR) all
	$(MAKE) -C $(DCMTK_DIR) install-lib 
	$(MAKE) -C $(DCMTK_DIR) install 

## Run Swig to generate csharp files and OffisWrapper
## BUILD_PATH is used as a directory because install tries to copy to the same directory as the source
#Offis_Wrapper.cxxdir=@SOURCE_PATH@/Dicom/OffisWrapper
Offis_Wrapper.cxxdir=@BUILD_PATH@
Offis_Wrapper.cxx_SCRIPTS=$(SWIG_OUTPUT_FILE)

SWIG_FLAGS = -csharp -c++ -DHAVE_CXX_BOOL -DHAVE_STD_STRING -DHAVE_CONFIG_H 

$(SWIG_OUTPUT_FILE) : $(SWIG_DEPEND) $(DCMTK_LIBS)
	rm -rf $(SWIG_OUTPUT_DIR)
	$(install_sh) -d $(SWIG_OUTPUT_DIR)
	$(SWIG) $(SWIG_FLAGS) \
		$(CXXFLAGS_INCLUDE) \
		-outdir $(SWIG_OUTPUT_DIR) \
		-Wall -makedefault \
		-o $(SWIG_OUTPUT_FILE) \
		-namespace $(projectNamespace) \
		$(SWIG_INTERFACE) 

## Generate native library from Offis_Wrapper.cxx
CXXFLAGS_PRE = -DSWIG -DWITH_LIBTIFF -DHAVE_CXX_BOOL -DHAVE_STD_STRING -DHAVE_CONFIG_H

CXXFLAGS_INCLUDE = \
		-I$(DCMTK_INCLUDE) \
		-I$(DCMTK_INCLUDE)/ofstd \
		-I$(DCMTK_INCLUDE)/dcmdata \
		-I$(DCMTK_INCLUDE)/dcmnet 

CXXFLAGS_LIBS = -L$(DCMTK_LIBS_DIR)

## Libtool procedure doesn't work use script generation below
#lib_LTLIBRARIES = $(NATIVE_FILE)
#libOffisDcm_ladir = $(NATIVE_OUTPUT_DIR)
#libOffisDcm_la_SOURCES = $(SWIG_OUTPUT_FILE)  
#libOffisDcm_la_CXXFLAGS = $(CXXFLAGS_PRE) $(CXXFLAGS_INCLUDE) $(CXXFLAGS_LIBS)
#libOffisDcm_la_LIBADD = $(NATIVE_FILE_LIBS)

libOffisDcm.sodir= $(NATIVE_OUTPUT_DIR)
libOffisDcm.so_SCRIPTS = $(NATIVE_FILE)

$(NATIVE_FILE) : $(SWIG_OUTPUT_FILE) 
	rm -rf $(NATIVE_OUTPUT_DIR)
	$(install_sh) -d $(NATIVE_OUTPUT_DIR)
	gcc -fpic -c $(CXXFLAGS_PRE) $(CXXFLAGS_INCLUDE) $(SWIG_OUTPUT_FILE) -o $(NATIVE_OUTPUT_DIR)/$(NATIVE_OBJECT)
	gcc -z defs -shared -o $(NATIVE_OUTPUT_DIR)/$(NATIVE_FILE) $(NATIVE_OUTPUT_DIR)/$(NATIVE_OBJECT) $(CXX_FLAGS_LIBS) $(NATIVE_FILE_LIBS)
	cp $(NATIVE_OUTPUT_DIR)/$(NATIVE_FILE) @BUILD_PATH@ 

## Compile CSharp and link to a Dll
ClearCanvas.Dicom.OffisWrapperdir=@SOURCE_PATH@/Dicom/OffisWrapper
ClearCanvas.Dicom.OffisWrapper_SCRIPTS=$(dllFilename)

$(dllFilename) : $(SWIG_OUTPUT_FILE) $(NATIVE_FILE)
	$(install_sh) -d $(SWIG_OUTPUT_DIR)/$(CONFIGURATION)
	$(install_sh) -d @BUILD_PATH@
	$(GMCS) -out:$(SWIG_OUTPUT_DIR)/$(CONFIGURATION)/$(dllFilename) \
		-target:library \
		-debug$(GMCS_DEBUG_FLAG) \
	 	$(SWIG_OUTPUT_DIR)/*.cs	
	cp $(SWIG_OUTPUT_DIR)/$(CONFIGURATION)/$(dllFilename) @BUILD_PATH@ 

clean-local :
	rm -rf $(SWIG_OUTPUT_DIR)/$(CONFIGURATION)
	rm -rf $(SWIG_OUTPUT_DIR)
	rm -rf $(DCMTK_BUILD_DIR)

install-exec-local :
	cp @BUILD_PATH@/$(dllFilename) @INSTALL_PATH@
	cp @BUILD_PATH@/$(NATIVE_FILE) @INSTALL_PATH@
