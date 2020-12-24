FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["BOTDemoApplication.Front/BOTDemoApplication.Front.csproj", "BOTDemoApplication.Front/"]
RUN dotnet restore "BOTDemoApplication.Front/BOTDemoApplication.Front.csproj"
COPY . .
WORKDIR "/src/BOTDemoApplication.Front"
RUN dotnet build "BOTDemoApplication.Front.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BOTDemoApplication.Front.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BOTDemoApplication.Front.dll"]