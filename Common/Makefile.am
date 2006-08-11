# Setting up variables
projectName=Common
projectNamespace=ClearCanvas.$(projectName)
dllFilename=$(projectNamespace).dll
loggingTool=log4net.dll
loggingConfig=logging.config
DEPENDENCIES=$(loggingTool)
REFERENCES=$(DEPENDENCIES)

# Files removed by the mostlyclean target of make
MOSTLYCLEANFILES=@BUILD_PATH@/$(dllFilename) @BUILD_PATH@/$(loggingTool)

# Directories and files included in the distribution
# Including directories will include all files including version control files
EXTRA_DIST=\
	refs \
	Tests \
	Mathematics \
	$(ClearCanvas.Common_sources) \
	ClearCanvas.* \
	SR.resx

# Setting up automake variables
ClearCanvas.Commondir=@SOURCE_PATH@/$(projectName)
ClearCanvas.Common.Mathematicsdir=@SOURCE_PATH@/$(projectName)/Mathematics
ClearCanvas.Common_SCRIPTS=$(dllFilename)
ClearCanvas.Common_sources = *.cs
ClearCanvas.Common.Mathematics_sources = *.cs
ClearCanvas.Common_build_sources=$(addprefix @srcdir@/, $(ClearCanvas.Common_sources))
ClearCanvas.Common_build_sources+=$(addprefix @srcdir@/Mathematics/, $(ClearCanvas.Common.Mathematics_sources))  

CSHARP_FLAGS =\
	-d:MONO,LINUX\
	-out:@BUILD_PATH@/$@\
	-target:library\
	-debug$(GMCS_DEBUG_FLAG)\
	-lib:@BUILD_PATH@\
	-pkg:dotnet\
	-resource:$(SR_RESOURCES),$(projectNamespace).$(SR_RESOURCES)\
	-reference:$(REFERENCES)

# Rules to make targets
$(loggingTool) :
	cp ./refs/$(loggingTool) @BUILD_PATH@

$(dllFilename) : $(DEPENDENCIES)
	sed -i '/System.Diagnostics/d' $(SR_DESIGNER)
	resgen $(SR_RESX)
	$(GMCS) $(CSHARP_FLAGS) $(ClearCanvas.Common_build_sources) 

install-exec-local : 
	sed -e 's/\\\\/\//g' @SOURCE_PATH@/Desktop/Executable/Logging.config > @INSTALL_PATH@/Logging.config
	cp @BUILD_PATH@/$(loggingTool) @INSTALL_PATH@
	cp @BUILD_PATH@/$(dllFilename) @INSTALL_PATH@
