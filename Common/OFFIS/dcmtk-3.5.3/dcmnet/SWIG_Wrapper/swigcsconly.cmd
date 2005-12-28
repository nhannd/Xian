@echo off
REM Parameter 1 - 'Debug' or 'Release'
REM Parameter 2 - '+' or '-'
del /q csharp\*
swig.exe -csharp -c++ -outdir csharp -Wall -namespace ClearCanvas.Dicom.Network.OffisWrapper dcmnet.i
copy ..\libsrc\SWIG%1\dcmnet.dll csharp
cd csharp
csc /out:sdcmnet.dll /target:library /debug%2 *.cs
cd ..

