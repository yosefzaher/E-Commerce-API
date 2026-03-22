#!/bin/bash

echo "[After Install Script] Applying Database Migrations..."

SECRET_JSON=$(aws secretsmanager get-secret-value --secret-id ClickToBuy/Prod/DbCredential --query SecretString --output text)

if [ -z "$SECRET_JSON" ]; then
    echo "[After Install Script] ERROR: Could not retrieve secret from Secrets Manager."
    exit 1
fi
DB_HOST=$(echo "$SECRET_JSON" | jq -r .host)
DB_USER=$(echo "$SECRET_JSON" | jq -r .username)
DB_PASS=$(echo "$SECRET_JSON" | jq -r .password)
DB_NAME="ClickToBuyDB"

MIGRATION_FILE=$(ls /home/ubuntu/E-Commerce-API/publish-output/migration_*.sql 2>/dev/null | head -n 1)

if [ -f "$MIGRATION_FILE" ]; then
    echo "[After Install Script] Found migration file: $MIGRATION_FILE. Executing on $DB_HOST..."

    /opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U "$DB_USER" -P "$DB_PASS" -d "$DB_NAME" -i "$MIGRATION_FILE" -C

    if [ $? -eq 0 ]; then
        echo "[After Install Script] Migration applied successfully!"
    else
        echo "[After Install Script] Migration FAILED! Blocking deployment."
        exit 1
    fi
else
    echo "[After Install Script] No migration file found in /publish-output/. Skipping..."
fi
        