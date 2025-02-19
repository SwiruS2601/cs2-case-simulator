FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /source

# Copy project files first
COPY --link *.csproj ./
COPY --link ../*.sln ./

# Clear NuGet cache and restore packages
RUN dotnet nuget locals all --clear && \
    dotnet restore "Cs2CaseOpener.csproj" -r linux-x64

# Copy the rest of the source code and database
COPY --link . ./

# Build and publish
RUN dotnet publish "Cs2CaseOpener.csproj" \
    -c Release \
    --runtime linux-x64 \
    --self-contained false \
    -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 8080
WORKDIR /app

ENV APP_UID=1000

# Create data directory and copy files
RUN mkdir -p /app/data
COPY --link --from=build /app .
COPY --link --from=build /source/cs2cases.db /app/data/cs2cases.db

# Set permissions
RUN chown -R $APP_UID:$APP_UID /app && \
    chmod 755 /app && \
    chmod 777 /app/data && \
    chmod 666 /app/data/cs2cases.db

ENV SQLITE_TMPDIR=/app/data
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/cs2cases.db"

USER $APP_UID
ENTRYPOINT ["./Cs2CaseOpener"]