#!/bin/bash

# this script installs az cli and logs into Azure

echo "$(date +'%Y-%m-%d %H:%M:%S')  azure.sh start" >> "$HOME/status"

if [ "$PIB_MI" != "" ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  installing az cli" >> "$HOME/status"

  sudo apt-get update
  curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

  echo "$(date +'%Y-%m-%d %H:%M:%S')  az login start" >> "$HOME/status"
  az login --identity -o table

  #shellcheck disable=2181
  if [ "$?" != 0 ]
  then
      echo "$(date +'%Y-%m-%d %H:%M:%S')  Azure login failed" >> "$HOME/status"
      echo "Azure login failed"
      exit 1
  fi

  echo "$(date +'%Y-%m-%d %H:%M:%S')  $(az account show -o tsv --query 'user.name')" >> "$HOME/status"

  if [ "$PIB_KEYVAULT" != "" ]
  then
    # save secrets
    key=$(az keyvault secret show --vault-name "$PIB_KEYVAULT" --query 'value' -o tsv -n ssl-crt)
    if [ "$key" != "" ]; then echo "$key" > "$HOME/.ssh/certs.pem"; fi

    key=$(az keyvault secret show --vault-name "$PIB_KEYVAULT" --query 'value' -o tsv -n ssl-key)
    if [ "$key" != "" ]; then echo "$key" > "$HOME/.ssh/certs.key"; fi

    key=$(az keyvault secret show --vault-name "$PIB_KEYVAULT" --query 'value' -o tsv -n fluent-bit-secret)
    if [ "$key" != "" ]; then echo "$key" > "$HOME/.ssh/fluent-bit.key"; fi

    key=$(az keyvault secret show --vault-name "$PIB_KEYVAULT" --query 'value' -o tsv -n prometheus-secret)
    if [ "$key" != "" ]; then echo "$key" > "$HOME/.ssh/prometheus.key"; fi

    key=$(az keyvault secret show --vault-name "$PIB_KEYVAULT" --query 'value' -o tsv -n event-hub-secret)
    if [ "$key" != "" ]; then echo "$key" > "$HOME/.ssh/event-hub.key"; fi

    echo "$(date +'%Y-%m-%d %H:%M:%S')  az login complete" >> "$HOME/status"
  fi
fi

echo "$(date +'%Y-%m-%d %H:%M:%S')  azure.sh complete" >> "$HOME/status"
