# Etapa 1: compilar con el SDK completo
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero solo el .csproj y el restore: si no cambian las dependencias,
# Docker reutiliza esta capa y el build es más rápido
COPY *.csproj ./
RUN dotnet restore

# Después el resto del código, y se publica en modo Release
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Etapa 2: imagen final, solo con el runtime (mucho más liviana)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# La app escucha en el puerto 8080
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "VegetableLine.dll"]