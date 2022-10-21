#!/bin/bash

if [ "$PIB_SSL" = "" ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  skipping dns setup" >> "$HOME/status"
  exit 0
fi

echo "$(date +'%Y-%m-%d %H:%M:%S')  dns start" >> "$HOME/status"

set -e

echo "$(date +'%Y-%m-%d %H:%M:%S')  dns start" >> "$HOME/status"
echo "$(date +'%Y-%m-%d %H:%M:%S')  creating DNS entry" >> "$HOME/status"

# get the public IP
pip=$(az network public-ip show -g "$PIB_RESOURCE_GROUP" -n "${PIB_CLUSTER}publicip" --query ipAddress -o tsv)

echo "$(date +'%Y-%m-%d %H:%M:%S')  Public IP: $pip" >> "$HOME/status"
echo "Public IP: $pip"

# get the old IP
old_ip=$(az network dns record-set a list \
--query "[?name=='$PIB_CLUSTER'].{IP:aRecords}" \
--resource-group "$PIB_DNS_RG" \
--zone-name "$PIB_SSL" \
-o json | jq -r '.[].IP[].ipv4Address' | tail -n1)

# delete old DNS entry if exists
if [ "$old_ip" != "" ] && [ "$old_ip" != "$pip" ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  deleting old DNS entry old: $old_ip pip: $pip" >> "$HOME/status"

  # delete the old DNS entry
  az network dns record-set a remove-record \
  -g "$PIB_DNS_RG" \
  -z "$PIB_SSL" \
  -n "$PIB_CLUSTER" \
  -a "$old_ip" -o table
fi

# create DNS record
az network dns record-set a add-record \
-g "$PIB_DNS_RG" \
-z "$PIB_SSL" \
-n "$PIB_CLUSTER" \
-a "$pip" \
--ttl 10 -o table

echo "$(date +'%Y-%m-%d %H:%M:%S')  dns complete" >> "$HOME/status"
