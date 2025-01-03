@echo off
echo -----------------------------------------------------------------
echo Publish schellenberg-web2rf-api service...
echo -----------------------------------------------------------------
docker tag schellenberg-web2rf-api atilladocker/schellenberg-web2rf-api:latest
docker push atilladocker/schellenberg-web2rf-api:latest
