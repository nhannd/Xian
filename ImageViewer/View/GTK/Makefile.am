projectName=ImageViewer.View.GTK
projectDir=ImageViewer/View/GTK
projectNamespace=ClearCanvas.$(projectName)
dllFilename=$(projectNamespace).dll

commaChar:= ,
empty:=
spaceChar:= $(empty) $(empty)
DEPENDENCIES=@BUILD_PATH@/ClearCanvas.ImageViewer.dll @BUILD_PATH@/ClearCanvas.Desktop.dll @BUILD_PATH@/ClearCanvas.Common.dll @BUILD_PATH@/ClearCanvas.Desktop.View.GTK.dll
REFERENCES=$(subst $(spaceChar),$(commaChar),$^)
PACKAGES=gtk-sharp-2.0,gtk-dotnet-2.0
MOSTLYCLEANFILES=@BUILD_PATH@/$(dllFilename) 
EXTRA_DIST=\
	$(ClearCanvas.ImageViewer.View.GTK_sources) \
	ClearCanvas* \
	SR.resx

ClearCanvas.ImageViewer.View.GTKdir=@SOURCE_PATH@/$(projectDir)
ClearCanvas.ImageViewer.View.GTK_SCRIPTS=$(dllFilename)
ClearCanvas.ImageViewer.View.GTK_sources = *.cs
ClearCanvas.ImageViewer.View.GTK_build_sources=$(addprefix @srcdir@/, $(ClearCanvas.ImageViewer.View.GTK_sources))

CSHARP_FLAGS =\
	-d:MONO,LINUX\
	-out:@BUILD_PATH@/$@\
	-target:library\
	-debug$(GMCS_DEBUG_FLAG)\
	-lib:@BUILD_PATH@\
	-pkg:dotnet\
	-resource:$(SR_RESOURCES),$(projectNamespace).$(SR_RESOURCES)\
	-reference:$(REFERENCES)\
	-pkg:$(PACKAGES)

$(dllFilename) : $(DEPENDENCIES)
	resgen $(SR_RESX)
	$(GMCS) $(CSHARP_FLAGS) $(ClearCanvas.ImageViewer.View.GTK_build_sources) 

install-exec-local:
	cp @BUILD_PATH@/$(dllFilename) @INSTALL_PLUGIN_PATH@
