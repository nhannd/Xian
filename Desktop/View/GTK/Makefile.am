projectName=Desktop.View.GTK
projectDir=Desktop/View/GTK
projectNamespace=ClearCanvas.$(projectName)
dllFilename=$(projectNamespace).dll

commaChar:= ,
empty:=
spaceChar:= $(empty) $(empty)
DEPENDENCIES=@BUILD_PATH@/ClearCanvas.ImageViewer.dll @BUILD_PATH@/ClearCanvas.Desktop.dll @BUILD_PATH@/ClearCanvas.Common.dll
REFERENCES=$(subst $(spaceChar),$(commaChar),$^)
PACKAGES=gtk-sharp-2.0,gtk-dotnet-2.0
MOSTLYCLEANFILES=@BUILD_PATH@/$(dllFilename) 
EXTRA_DIST=\
	Icons \
	$(ClearCanvas.Desktop.View.GTK_sources) \
	ClearCanvas.* \
	SR.resx \
	SR.txt

ClearCanvas.Desktop.View.GTKdir=@SOURCE_PATH@/$(projectDir)
ClearCanvas.Desktop.View.GTK_SCRIPTS=$(dllFilename)
ClearCanvas.Desktop.View.GTK_sources = *.cs
ClearCanvas.Desktop.View.GTK_build_sources=$(addprefix @srcdir@/, $(ClearCanvas.Desktop.View.GTK_sources))

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
	$(ICON_FLAGS)\
	-reference:$(REFERENCES)\
	-pkg:$(PACKAGES)

$(dllFilename) : $(DEPENDENCIES)
	resgen $(SR_RESX)
	$(GMCS) $(CSHARP_FLAGS) $(ClearCanvas.Desktop.View.GTK_build_sources) 

install-exec-local :
	cp @BUILD_PATH@/$(dllFilename) @INSTALL_PLUGIN_PATH@
