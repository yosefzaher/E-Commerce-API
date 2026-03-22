#!/bin/bash

echo "[Launch Template INFO] Starting Launch Template Script......." 

# Update the Server
echo "[Launch Template INFO] Updating The Server......." 
sudo apt-get update -y
sudo apt-get install -y unzip jq curl gnupg2 software-properties-common

# Download .NET 8.0 Runtime
echo "[Launch Template INFO] Downloading .NET 8.0 Runtime......." 
sudo apt-get install -y aspnetcore-runtime-8.0

# Install AWS CLI v2
echo "[Launch Template INFO] Installing AWS CLI v2......." 
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip -o awscliv2.zip
sudo ./aws/install --update
rm -rf awscliv2.zip aws/

# Install mssql-tools18
echo "[Launch Template INFO] Installing mssql-tools18......." 
curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | sudo gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg

echo "deb [arch=amd64,arm64,armhf signed-by=/usr/share/keyrings/microsoft-prod.gpg] https://packages.microsoft.com/ubuntu/24.04/prod noble main" | sudo tee /etc/apt/sources.list.d/mssql-release.list

# Update and install mssql-tools18 and unixodbc-dev
sudo apt-get update
sudo ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev

# Adding the path to the system-wide PATH
echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' | sudo tee -a /etc/profile.d/mssql-tools.sh
source /etc/profile.d/mssql-tools.sh

# install CodeDeploy Agent
echo "[Launch Template INFO] Installing CodeDeploy Agent......."
sudo apt-get install -y ruby-full
sudo apt-get install -y wget
cd /home/ubuntu
wget https://aws-codedeploy-us-east-1.s3.us-east-1.amazonaws.com/latest/install
chmod +x ./install
sudo ./install auto
sudo systemctl enable codedeploy-agent
sudo systemctl start codedeploy-agent

echo "[Launch Template INFO] Environment Setup Completed Successfully!"