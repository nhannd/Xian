projectName=ImageViewer.Layout.Basic.View.GTK
projectDir=ImageViewer/Layout/Basic/View/GTK
projectNamespace=ClearCanvas.$(projectName)
dllFilename=$(projectNamespace).dll

commaChar:= ,
empty:=
spaceChar:= $(empty) $(empty)
DEPENDENCIES=@BUILD_PATH@/ClearCanvas.ImageViewer.dll @BUILD_PATH@/ClearCanvas.Desktop.dll @BUILD_PATH@/ClearCanvas.Common.dll @BUILD_PATH@/ClearCanvas.ImageViewer.Layout.Basic.dll @BUILD_PATH@/ClearCanvas.ImageViewer.View.GTK.dll
REFERENCES=$(subst $(spaceChar),$(commaChar),$^)
PACKAGES=gtk-sharp-2.0
MOSTLYCLEANFILES=@BUILD_PATH@/$(dllFilename) 
EXTRA_DIST=\
	$(ClearCanvas.ImageViewer.Layout.Basic.View.GTK_sources) \
	ClearCanvas* 

ClearCanvas.ImageViewer.Layout.Basic.View.GTKdir=@SOURCE_PATH@/$(projectDir)
ClearCanvas.ImageViewer.Layout.Basic.View.GTK_SCRIPTS=$(dllFilename)
ClearCanvas.ImageViewer.Layout.Basic.View.GTK_sources = *.cs
ClearCanvas.ImageViewer.Layout.Basic.View.GTK_build_sources=$(addprefix @srcdir@/, $(ClearCanvas.ImageViewer.Layout.Basic.View.GTK_sources))

CSHARP_FLAGS =\
	-d:MONO,LINUX\
	-out:@BUILD_PATH@/$@\
	-target:library\
	-debug$(GMCS_DEBUG_FLAG)\
	-lib:@BUILD_PATH@\
	-pkg:dotnet\
	-reference:$(REFERENCES)\
	-pkg:$(PACKAGES)

$(dllFilename) : $(DEPENDENCIES)
	$(GMCS) $(CSHARP_FLAGS) $(ClearCanvas.ImageViewer.Layout.Basic.View.GTK_build_sources) 

install-exec-local:
	cp @BUILD_PATH@/$(dllFilename) @INSTALL_PLUGIN_PATH@
