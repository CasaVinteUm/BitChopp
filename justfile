#!/usr/bin/env -S just --justfile

default:
  @{{just_executable()}} --list

build:
    dotnet build

# Builds the release executable
publish:
    dotnet clean
    dotnet publish -c Release -r linux-arm --self-contained true \
        -p:PublishReadyToRun=true \
        -p:PublishSingleFile=true \
        -p:IncludeNativeLibrariesForSelfExtract=true

# Add key variable to enable using non-default key to connect to the RPI
key := ""

# Build and copy to the RPI
deploy:
    rsync -avz -e "$(test -n "{{key}}" && echo "ssh -i {{key}}" || echo "ssh")" --progress src/BitChopp.Main/bin/Release/net8.0/linux-arm/publish/* pi@casa21chopp.local:/home/pi/BitChopp

# Runs the app on the RPI
run-remote:
    ssh $(test -n "{{key}}" && echo "-i {{key}}") pi@casa21chopp.local -C ./BitChopp.sh
