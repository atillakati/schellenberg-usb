@echo off
echo -----------------------------------------------------------------
echo Publish schellenberg-web2rf-api service...
echo -----------------------------------------------------------------
docker tag schellenberg-web2rf-api atilladocker/schellenberg-web2rf-api:0.0.3
docker push atilladocker/schellenberg-web2rf-api:0.0.3
