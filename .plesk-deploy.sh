#!/bin/bash

# Define paths
APP_DIR="$DEPLOYMENT_TARGET"
BACKEND_DIR="$APP_DIR"
FRONTEND_DIR="$APP_DIR/deloprosit.client"
PUBLISH_DIR="$APP_DIR/publish"

# Install .NET Core if needed
if ! command -v dotnet &> /dev/null; then
    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --runtime aspnetcore --version 8.0.0
    export PATH="$HOME/.dotnet:$PATH"
fi

# Build and publish ASP.NET Core backend
cd "$BACKEND_DIR"
dotnet restore
dotnet publish -c Release -o "$PUBLISH_DIR"

# Build Vue.js frontend
cd "$FRONTEND_DIR"
npm install
npm run build
mv dist "$PUBLISH_DIR/wwwroot"

# Set permissions
chmod -R 755 "$PUBLISH_DIR"