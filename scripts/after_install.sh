#!/bin/bash

# After Install Script
echo "[After Install Script] Starting After Install Phase..."

# Setting Permissions
echo "[After Install Script] Setting permissions..."
chown -R ubuntu:ubuntu /home/ubuntu/E-Commerce-API
chmod +x /home/ubuntu/E-Commerce-API/publish-output/ECommerce-API.dll


# Making a Service From the .NET Project
echo "[After Install Script] Making a Service From the .NET Project......."
cat >/etc/systemd/system/ecommerce_api.service <<EOL
[Unit]
Description=.NET E-Commerce API Server Work on Port 5253

[Service]
ExecStart=/usr/bin/dotnet /home/ubuntu/E-Commerce-API/publish-output/ECommerce-API.dll --urls "http://0.0.0.0:5253"
SyslogIdentifier=ecommerce_api
WorkingDirectory=/home/ubuntu/E-Commerce-API/publish-output/
User=ubuntu
Environment=DOTNET_CLI_HOME=/tmp


[Install]
WantedBy=multi-user.target
EOL

# Reload Systemd Daemon
echo "[After Install Script] Reloading Systemd Daemon...."
systemctl daemon-reload
systemctl enable ecommerce_api
systemctl restart ecommerce_api

echo "[After Install Script] After Install Phase Completed."