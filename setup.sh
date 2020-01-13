#!/bin/bash

#Where the action happens
mkdir -p ~/Videos/EncodeWorker/jobs/completed
mkdir -p ~/Videos/EncodeWorker/active
mkdir -p ~/Videos/EncodeWorker/input

#Config folder goodness
sudo mkdir -p /var/local/EncodeWorker
sudo -n cp EncodeJobIngester/global.config /var/local/EncodeWorker/ingester-xml.config
sudo -n cp EncodeJobOverseer/global.config /var/local/EncodeWorker/overseer-xml.config

cd InputSplitter && dotnet publish -c Release && \
sudo cp bin/Release/netcoreapp3.0/ubuntu.18.04-x64/publish/InputSplitter /usr/local/bin/

cd ../EncodeJobIngester && dotnet publish -c Release && \
sudo cp bin/Release/netcoreapp3.0/ubuntu.18.04-x64/publish/EncodeJobIngester /usr/local/bin/

cd ../EncodeJobOverseer && dotnet public -c Release && \
sudo cp bin/Release/netcoreapp3.0/ubuntu.18.04-x64/publish/EncodeJobOverseer /usr/local/bin/