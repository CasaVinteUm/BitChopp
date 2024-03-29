# BitChopp

Casa21's BitChopp is a simple GUI that allows us to sell draft beer.

## Running

To start the project locally, run:

```bash
dotnet run
```

## Building

To build the project to be executed on a RaspiOS, run:

```bash
dotnet clean && dotnet publish -c Release -r linux-arm -p:PublishReadyToRun=true -p:PublishSingleFile=true --self-contained true -p:IncludeNativeLibrariesForSelfExtract=true
```

## Copying to Raspberry Pi

To copy the project to a Raspberry Pi, run:

```bash
# If you're using your default key:
rsync -avz --progress bin/Release/net8.0/linux-arm/publish/* pi@casa21chopp.local:/home/pi/BitChopp

# If you're using a custom key:
rsync -avz --progress -e "ssh -i ssh_path/your_key" bin/Release/net8.0/linux-arm/publish/* pi@casa21chopp.local:/home/pi/BitChopp
```