@echo off
echo -----------------------------------------------------------------
echo Publish schellenberg-web2rf-api service...
echo -----------------------------------------------------------------
docker tag schellenberg-web2rf-api:0.0.14 atilladocker/schellenberg-web2rf-api:0.0.14
docker push atilladocker/schellenberg-web2rf-api:0.0.14
