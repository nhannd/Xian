projectName=ImageViewer.Layout.Basic
projectDir=ImageViewer/Layout/Basic
projectNamespace=ClearCanvas.$(projectName)
dllFilename=$(projectNamespace).dll

commaChar:= ,
empty:=
spaceChar:= $(empty) $(empty)
DEPENDENCIES=@BUILD_PATH@/ClearCanvas.ImageViewer.dll @BUILD_PATH@/ClearCanvas.Desktop.dll @BUILD_PATH@/ClearCanvas.Common.dll
REFERENCES=$(subst $(spaceChar),$(commaChar),$^)
MOSTLYCLEANFILES=@BUILD_PATH@/$(dllFilename) 
EXTRA_DIST= \
	Icons \
	Tests \
	View/WinForms \
	$(ClearCanvas.ImageViewer.Layout.Basic_sources) \
	ClearCanvas* \
	SR.resx	

ClearCanvas.ImageViewer.Layout.Basicdir=@SOURCE_PATH@/$(projectDir)
ClearCanvas.ImageViewer.Layout.Basic_SCRIPTS=$(dllFilename)
ClearCanvas.ImageViewer.Layout.Basic_sources = *.cs
ClearCanvas.ImageViewer.Layout.Basic_build_sources=$(addprefix @srcdir@/, $(ClearCanvas.ImageViewer.Layout.Basic_sources))

icons = $(wildcard Icons/*.png)
ICON_FLAGS = $(foreach icon,$(icons),-resource:$(icon)$(commaChar)$(projectNamespace).$(subst /,.,$(icon)))

CSHARP_FLAGS =\
	-d:MONO,LINUX\
	-out:@BUILD_PATH@/$@\
	-target:library\
	-debug$(GMCS_DEBUG_FLAG)\
	-lib:@BUILD_PATH@\
	-pkg:dotnet\
	-resource:$(SR_RESOURCES),$(projectNamespace).$(SR_RESOURCES)\
	$(ICON_FLAGS) \
	-reference:$(REFERENCES)

$(dllFilename) : $(DEPENDENCIES)
	sed -i '/System.Diagnostics/d' $(SR_DESIGNER)
	resgen $(SR_RESX)
	$(GMCS) $(CSHARP_FLAGS) $(ClearCanvas.ImageViewer.Layout.Basic_build_sources) 

install-exec-local:
	cp @BUILD_PATH@/$(dllFilename) @INSTALL_PLUGIN_PATH@
