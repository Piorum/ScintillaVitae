#ScintillaVitae.Database
FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /App

COPY ScintillaVitae.Database/. ./ScintillaVitae.Database/
COPY SharedProtos/. ./SharedProtos/

WORKDIR /App/ScintillaVitae.Database

RUN dotnet restore 
RUN dotnet publish -o out

FROM mcr.microsoft.com/dotnet/aspnet
WORKDIR /App
COPY --from=build /App/ScintillaVitae.Database/out .
ENTRYPOINT [ "dotnet", "ScintillaVitae.Database.dll" ]