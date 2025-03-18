@echo off
echo -----------------------------------------------------------------
echo Publish schellenberg-web2rf-api service...
echo -----------------------------------------------------------------
docker tag schellenberg-web2rf-api:%1 atilladocker/schellenberg-web2rf-api:%1
docker push atilladocker/schellenberg-web2rf-api:%1
