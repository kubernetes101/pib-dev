#!/bin/bash

echo "$(date +'%Y-%m-%d %H:%M:%S')  flux bootstrap start" >> "$HOME/status"

# inner-loop
if [ "$PIB_IS_INNER_LOOP" = "true" ]; then
  exit 0
fi

# setup Arc enabled flux
if [ "$PIB_ARC_ENABLED" = "true" ]
then
  exit 0
fi

# make sure flux is installed
if [ ! "$(flux --version)" ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  flux not found" >> "$HOME/status"
  echo "$(date +'%Y-%m-%d %H:%M:%S')  flux bootstrap failed" >> "$HOME/status"
  exit 1
fi

# make sure the branch is set
if [ -z "$PIB_BRANCH" ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  PIB_BRANCH not set" >> "$HOME/status"
  echo "$(date +'%Y-%m-%d %H:%M:%S')  flux bootstrap failed" >> "$HOME/status"
  echo "PIB_BRANCH not set"
  exit 1
fi

# make sure cluster name is set
if [ -z "$PIB_CLUSTER" ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  PIB_CLUSTER not set" >> "$HOME/status"
  echo "$(date +'%Y-%m-%d %H:%M:%S')  flux bootstrap failed" >> "$HOME/status"
  echo "PIB_CLUSTER not set"
  exit 1
fi

# make sure PAT is set
if [ "$PIB_PAT" = "" ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  PIB_PAT not set" >> "$HOME/status"
  echo "$(date +'%Y-%m-%d %H:%M:%S')  flux bootstrap failed" >> "$HOME/status"
  echo "PIB_PAT not set"
  exit 1
fi

git pull

kubectl apply -f "$HOME/pib/clusters/$PIB_CLUSTER/flux-system/namespace.yaml"
flux create secret git flux-system -n flux-system --url "$PIB_FULL_REPO" -u gitops -p "$PIB_PAT"
flux create secret git gitops -n flux-system --url "$PIB_FULL_REPO" -u gitops -p "$PIB_PAT"

kubectl apply -f "$HOME/pib/clusters/$PIB_CLUSTER/flux-system/controllers.yaml"
sleep 3
kubectl apply -f "$HOME/pib/clusters/$PIB_CLUSTER/flux-system/source.yaml"
sleep 2
kubectl apply -R -f "$HOME/pib/clusters/$PIB_CLUSTER/flux-system"
sleep 5

# force flux to sync
flux reconcile source git gitops

# display results
kubectl get pods -A
flux get kustomizations

echo "$(date +'%Y-%m-%d %H:%M:%S')  flux bootstrap complete" >> "$HOME/status"
