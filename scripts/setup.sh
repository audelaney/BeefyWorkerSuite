#!/bin/bash

#execute from top project directory

#Where the action happens
mkdir -p ~/Videos/EncodeWorker/jobs/completed
mkdir -p ~/Videos/EncodeWorker/jobs/active
mkdir -p ~/Videos/EncodeWorker/jobs/failed
mkdir -p ~/Videos/EncodeWorker/jobs/processed
mkdir -p ~/Videos/EncodeWorker/input

#Config folder goodness
cp -n AppConfig/default.config ~/Videos/EncodeWorker/local.overseer.config
cp -n AppConfig/default.config ~/Videos/EncodeWorker/local.ingester.config

cd MasterApp && dotnet publish -c Release && \
sudo cp bin/Release/netcoreapp3.0/ubuntu.18.04-x64/publish/MasterApp /usr/local/bin/

cd ../EncodeJobIngester && dotnet publish -c Release && \
sudo cp bin/Release/netcoreapp3.0/ubuntu.18.04-x64/publish/EncodeJobIngester /usr/local/bin/

cd ../EncodeJobOverseer && dotnet publish -c Release && \
sudo cp bin/Release/netcoreapp3.0/ubuntu.18.04-x64/publish/EncodeJobOverseer /usr/local/bin/