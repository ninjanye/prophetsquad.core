FROM microsoft/dotnet:1.1.0-runtime
WORKDIR /app
ENTRYPOINT ["dotnet", "ProphetSquad.Core.Updater.dll"]
ENV bfUser=xxxxxx
ENV bfPassword=xxxxxx
COPY release /app
