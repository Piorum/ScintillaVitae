#ScintillaVitae
FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /App

COPY ScintillaVitae/. ./ScintillaVitae/
COPY SharedProtos/. ./SharedProtos/

WORKDIR /App/ScintillaVitae

RUN dotnet restore 
RUN dotnet publish -o out

FROM mcr.microsoft.com/dotnet/runtime
WORKDIR /App
COPY --from=build /App/ScintillaVitae/out .
ENTRYPOINT [ "dotnet", "ScintillaVitae.dll" ]