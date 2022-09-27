#!/usr/bin/env bash

# change to this directory
cd "$(dirname "${BASH_SOURCE[0]}")" || exit

set -e

echo "$(date +'%Y-%m-%d %H:%M:%S')  skipping arc-setup" >> "$HOME/status"

if [ "$PIB_ARC_ENABLED" != "true" ]; then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  arc-setup complete" >> "$HOME/status"
  exit 0
fi

# add azure arc dependencies
echo "$(date +'%Y-%m-%d %H:%M:%S')  install Arc dependencies" >> "$HOME/status"
az extension add --name connectedk8s
az extension add --name k8s-configuration
az extension add --name  k8s-extension
az provider register --namespace Microsoft.Kubernetes
az provider register --namespace Microsoft.KubernetesConfiguration
az provider register --namespace Microsoft.ExtendedLocation

# make sure cluster name is set
if [ -z "$PIB_CLUSTER" ]; then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  PIB_CLUSTER not set" >> "$HOME/status"
  echo "$(date +'%Y-%m-%d %H:%M:%S')  arc setup failed" >> "$HOME/status"
  echo "PIB_CLUSTER not set"
  exit 1
fi

# make sure resource group is set
if [ -z "$PIB_RESOURCE_GROUP" ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  PIB_RESOURCE_GROUP not set" >> "$HOME/status"
  echo "$(date +'%Y-%m-%d %H:%M:%S')  arc setup failed" >> "$HOME/status"
  echo "PIB_CLUSTER not set"
  exit 1
fi

# make sure the branch is set
if [ -z "$PIB_BRANCH" ]; then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  PIB_BRANCH not set" >> "$HOME/status"
  echo "$(date +'%Y-%m-%d %H:%M:%S')  arc setup failed" >> "$HOME/status"
  echo "PIB_BRANCH not set"
  exit 1
fi

# make sure the branch is set
if [ -z "$PIB_PAT" ]; then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  PIB_PAT not set" >> "$HOME/status"
  echo "$(date +'%Y-%m-%d %H:%M:%S')  arc setup failed" >> "$HOME/status"
  echo "PIB_BRANCH not set"
  exit 1
fi

# connect K8s to Arc
echo "$(date +'%Y-%m-%d %H:%M:%S')  Arc enable cluster" >> "$HOME/status"
echo "Arc enable cluster"
az connectedk8s connect --name "$PIB_CLUSTER" --resource-group "$PIB_RESOURCE_GROUP"

echo "$(date +'%Y-%m-%d %H:%M:%S')  Arc enable GitOps" >> "$HOME/status"
echo "Arc enable GitOps"

# add flux extension
az k8s-configuration flux create \
  --cluster-type connectedClusters \
  --interval 1m \
  --kind git \
  --name gitops \
  --namespace flux-system \
  --scope cluster \
  --timeout 3m \
  --https-user gitops \
  --cluster-name "$PIB_CLUSTER" \
  --resource-group "$PIB_RESOURCE_GROUP" \
  --url "$PIB_FULL_REPO" \
  --branch "$PIB_BRANCH" \
  --https-key "$PIB_PAT" \
  --kustomization \
      name=flux-system \
      path=./clusters/"$PIB_CLUSTER"/flux-system/listeners \
      timeout=3m \
      sync_interval=1m \
      retry_interval=1m \
      prune=true \
      force=true

# setup Key Vault
if [ "$PIB_KEYVAULT" != "" ]; then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  Arc-enabled key vault" >> "$HOME/status"
  # add key vault extension
  az k8s-extension create \
    --cluster-type connectedClusters \
    --extension-type Microsoft.AzureKeyVaultSecretsProvider \
    --cluster-name "$PIB_CLUSTER" \
    --resource-group "$PIB_RESOURCE_GROUP" \
    --name "$PIB_KEYVAULT"
fi

if [ "$PIB_OSM_ENABLED" = "true" ]; then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  Arc-enabled OSM" >> "$HOME/status"

cat <<EOF > osm.json
{
  "osm.osm.osmNamespace" : "arc-osm-system",
  "osm.contour.enabled" : "true",
  "osm.contour.configInline.tls.envoy-client-certificate.name" : "osm-contour-envoy-client-cert",
  "osm.contour.configInline.tls.envoy-client-certificate.namespace" : "arc-osm-system"
}
EOF

  az k8s-extension create \
    --cluster-type connectedClusters \
    --extension-type Microsoft.openservicemesh \
    --scope cluster \
    --cluster-name "$PIB_CLUSTER" \
    --resource-group "$PIB_RESOURCE_GROUP" \
    --name osm \
    --configuration-settings-file osm.json

  # Install contour
  echo "$(date +'%Y-%m-%d %H:%M:%S')  installing contour" >> "$HOME/status"
  kubectl apply -f contour.yaml
  sleep 5
  kubectl wait pod -l app=contour -n projectcontour --for condition=ready --timeout 30s

  kubectl apply -f cert-manager.yaml
  sleep 5
  kubectl wait pod -l app=webhook -n cert-manager --for condition=ready --timeout 30s

  # setup let's encrypt
  if [ "$PIB_SSL" != "" ]; then
    kubectl apply -f lets-encrypt.yaml
  fi

  # setup Dapr
  if [ "$PIB_DAPR_ENABLED" = "true" ]; then
    echo "$(date +'%Y-%m-%d %H:%M:%S')  installing dapr" >> "$HOME/status"
    wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | sudo /bin/bash
    sudo dapr init -k --enable-mtls=false --wait
  fi
fi

echo "$(date +'%Y-%m-%d %H:%M:%S')  arc-setup complete" >> "$HOME/status"
