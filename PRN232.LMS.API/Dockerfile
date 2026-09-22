FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY PRN232.LMS.API/PRN232.LMS.API.csproj PRN232.LMS.API/
COPY PRN232.LMS.Services/PRN232.LMS.Services.csproj PRN232.LMS.Services/
COPY PRN232.LMS.Repositories/PRN232.LMS.Repositories.csproj PRN232.LMS.Repositories/
RUN dotnet restore PRN232.LMS.API/PRN232.LMS.API.csproj
COPY PRN232.LMS.API/ PRN232.LMS.API/
COPY PRN232.LMS.Services/ PRN232.LMS.Services/
COPY PRN232.LMS.Repositories/ PRN232.LMS.Repositories/
RUN dotnet publish PRN232.LMS.API/PRN232.LMS.API.csproj -c Release -o /app --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER app
ENTRYPOINT ["dotnet", "PRN232.LMS.API.dll"]
