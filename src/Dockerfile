# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UsbDataTransmitter.Service/UsbDataTransmitter.Service.csproj", "UsbDataTransmitter.Service/"]
RUN dotnet restore "./UsbDataTransmitter.Service/UsbDataTransmitter.Service.csproj"
COPY . .
WORKDIR "/src/UsbDataTransmitter.Service"
RUN dotnet build "./UsbDataTransmitter.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UsbDataTransmitter.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-arm64v8 AS final

# Copy the entrypoint script
COPY installscript.sh /installscript.sh
RUN chmod +x /installscript.sh

WORKDIR /app
COPY --from=publish /app/publish .

# Set the entrypoint
ENTRYPOINT ["/installscript.sh"]

# Set the default command
CMD ["dotnet", "UsbDataTransmitter.Service.dll"]
# ENTRYPOINT ["dotnet", "UsbDataTransmitter.Service.dll"]

