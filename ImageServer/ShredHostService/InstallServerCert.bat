@echo Installing self-signed certificate for ImageServer
@echo  You only need to do on this machine once.
@echo.

@makecert -sr LocalMachine -ss My -a sha1 -n CN=ImageServer -sky exchange -pe  ImageServer.cer

@echo Server certificate ImageServer.cer is created.
@echo. 
@echo. 
@echo It has been imported into the LocalMachine Personal store of the server.
@echo.
@echo If the client needs to authenticate server based on this certificate, import this certificate into the client Trusted People store.
