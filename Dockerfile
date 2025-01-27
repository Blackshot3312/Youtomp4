# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copie o arquivo .csproj e restaure as dependências
COPY ["src/Api/Api.csproj", "src/Api/"]
RUN dotnet restore "src/Api/Api.csproj"

# Copie todos os outros arquivos
COPY src .

# Defina o diretório de trabalho para /src/Api
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# Estágio 2: Publish
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

# Estágio 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Instale cliente MariaDB (compatível com MySQL)
RUN apt-get update && apt-get install -y mariadb-client && apt-get clean

COPY --from=publish /app/publish .

# Defina o ponto de entrada do aplicativo
ENTRYPOINT ["dotnet", "Api.dll"]
