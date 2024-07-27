# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY dynamicarecord.csproj ./
RUN dotnet restore

# Copy the rest of the application and build it
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Copy the config.yml file
COPY config.yml /app/config.yml

# Install cron
RUN apt-get update && apt-get install -y cron

# Add crontab file in the cron directory
ADD crontab /etc/cron.d/dynamicarecord-cron

# Give execution rights on the cron job
RUN chmod 0644 /etc/cron.d/dynamicarecord-cron

# Apply cron job
RUN crontab /etc/cron.d/dynamicarecord-cron

# Create the log file to be able to run tail
RUN touch /var/log/cron.log

# Run the command on container startup
CMD cron && tail -f /var/log/cron.log