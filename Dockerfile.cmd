# build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# copy everything into the current folder
COPY . .

# publish the project in release mode
RUN dotnet publish ./paste.bin.ingest.cmd/paste.bin.ingest.cmd.csproj -c release

# final/running stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

# create and copy build files into paste.bin.ingest.cmd folder
WORKDIR /bin/paste.bin.ingest
COPY --from=build /src/paste.bin.ingest.cmd/bin/release/net6.0/publish .

# copy the crontab file
COPY --from=build /src/paste.bin.ingest.cmd.crontab .

# install cron
RUN apt-get update && apt-get -y install cron

# save crontab as job
RUN crontab /bin/paste.bin.ingest/paste.bin.ingest.cmd.crontab

# run cron on startup
ENTRYPOINT [ "cron", "-f" ]
