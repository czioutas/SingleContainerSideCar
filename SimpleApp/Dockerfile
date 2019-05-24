FROM microsoft/dotnet:2.2.100-sdk-alpine3.8 AS build
WORKDIR /SimpleApp

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

# Copy csproj and restore as distinct layers
COPY /SimpleApp/SimpleApp.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY /SimpleApp /SimpleApp
RUN dotnet publish -c $BUILDCONFIG -o out /p:Version=$VERSION

# Build runtime image
FROM microsoft/dotnet:2.2.0-aspnetcore-runtime-alpine3.8
WORKDIR /SimpleApp
COPY --from=build /SimpleApp/out .

EXPOSE 5000
# CMD ["dotnet", "SimpleApp.dll"]
ENTRYPOINT ["dotnet", "SimpleApp.dll"]