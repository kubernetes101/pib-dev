#!/bin/bash

# change to this directory
cd "$(dirname "${BASH_SOURCE[0]}")" || exit

echo "$(date +'%Y-%m-%d %H:%M:%S')  k3d-setup start" >> "$HOME/status"

if [ "$PIB_IS_INNER_LOOP" = "true" ]; then
  "$HOME/bin/kic" cluster create

  echo "$(date +'%Y-%m-%d %H:%M:%S')  k3d-setup complete" >> "$HOME/status"

  exit 0
fi

# fail if k3d.yaml isn't present
if [ ! -f ./k3d.yaml ]
then
  echo "failed (k3d.yaml not found)"
  exit 1
fi

# this will fail harmlessly if a cluster doesn't exist
k3d cluster delete

echo "$(date +'%Y-%m-%d %H:%M:%S')  transforming registries.yaml" >>"$HOME/status"
cp ./registries.templ /home/pib/registries.yaml
sed -i -e "s/{{pib-pat}}/$PIB_PAT/g" /home/pib/registries.yaml

# create the cluster (run as pib)
k3d cluster create \
  --registry-use k3d-registry.localhost:5500 \
  --registry-config /home/pib/registries.yaml \
  --config ./k3d.yaml \
  --k3s-arg "--disable=servicelb@server:0" \
  --k3s-arg "--disable=traefik@server:0"

# sleep to avoid timing issues
sleep 5
kubectl wait node --all  --for condition=ready --timeout 30s
sleep 5
kubectl wait pod -l k8s-app=kube-dns -n kube-system --for condition=ready --timeout 30s

if [ "$PIB_OSM_ENABLED" != "true" ]; then
  # Install contour
  echo "$(date +'%Y-%m-%d %H:%M:%S')  installing contour" >> "$HOME/status"
  kubectl apply -f contour.yaml
  sleep 5
  kubectl wait pod -l app=contour -n projectcontour --for condition=ready --timeout 30s

  # setup let's encrypt
  if [ "$PIB_SSL" != "" ]
  then
    kubectl apply -f cert-manager.yaml
    sleep 5
    kubectl wait pod -l app=webhook -n cert-manager --for condition=ready --timeout 30s

    kubectl apply -f lets-encrypt.yaml
  fi
fi

echo "$(date +'%Y-%m-%d %H:%M:%S')  k3d-setup complete" >> "$HOME/status"
