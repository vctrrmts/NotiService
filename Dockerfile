FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проекта
COPY ["NotiService/NotiService.csproj", "NotiService/"]
RUN dotnet restore "NotiService/NotiService.csproj"
COPY . .
WORKDIR "/src/NotiService"
RUN dotnet build "NotiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotiService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "NotiService.dll"] 