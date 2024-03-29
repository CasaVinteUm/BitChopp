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

# Build and copy to the RPI
deploy: publish
    rsync -avz --progress bin/Release/net8.0/linux-arm/publish/* pi@casa21chopp.local:/home/pi/BitChopp

# Runs the app on the RPI
run-remote:
    ssh pi@casa21chopp.local -C ./BitChopp.sh
