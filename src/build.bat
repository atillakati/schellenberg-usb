@echo off
REM echo -----------------------------------------------------------------
REM echo Stopping schellenberg-web2rf-api service...
REM echo -----------------------------------------------------------------
REM docker-compose -f .\docker-compose.yaml -p aksoft-energymeter-service down

echo -----------------------------------------------------------------
echo Building schellenberg-web2rf-api service...
echo -----------------------------------------------------------------
docker buildx build --platform linux/arm64 -t schellenberg-web2rf-api:latest .