FROM microsoft/dotnet:2.2.100-sdk-alpine3.8 AS build
WORKDIR /SideCar

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

# Copy csproj and restore as distinct layers
COPY /SideCar/SideCar.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY /SideCar /SideCar
RUN dotnet publish -c $BUILDCONFIG -o out /p:Version=$VERSION

# Build runtime image
FROM microsoft/dotnet:2.2.0-aspnetcore-runtime-alpine3.8
WORKDIR /SideCar
COPY --from=build /SideCar/out .

# CMD ["dotnet", "SideCar.dll"]
ENTRYPOINT ["dotnet", "SideCar.dll"]