FROM mcr.microsoft.com/dotnet/sdk:3.1 as build

WORKDIR /src

COPY ./grpcserver/grpcserver.csproj ./grpcserver/

RUN dotnet restore "grpcserver/grpcserver.csproj"

COPY . .

RUN dotnet publish "grpcserver/grpcserver.csproj" -c Release -o /published

FROM mcr.microsoft.com/dotnet/aspnet:3.1 as runtime

# Uncomment the line below if running with HTTPS
#ENV ASPNETCORE_URLS=https://+:443

WORKDIR /app

COPY --from=build /published .

ENTRYPOINT [ "dotnet", "grpcserver.dll" ]