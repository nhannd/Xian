@echo off
REM Parameter 1 - 'Debug' or 'Release'
REM Parameter 2 - '+' or '-'
del /q csharp\*
swig.exe -csharp -c++ -outdir csharp -Wall -namespace ClearCanvas.Dicom.Network.OffisWrapper dcmnet.i
copy dcmnet_wrap.cxx ..\libsrc
del /q ..\libsrc\swig%1
rem devenv ..\libsrc\dcmnet_wwrapper.sln /build Release
devenv.com ..\libsrc\dcmnet_wwrapper.sln /build %1
copy ..\libsrc\SWIG%1\dcmnet.dll csharp
rem copy ..\libsrc\swig%1\dcmnet.dll Test\bin\%1
cd csharp
csc /out:sdcmnet.dll /target:library /debug%2 *.cs
cd ..

