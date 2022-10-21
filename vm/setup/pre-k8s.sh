#!/bin/bash

# this runs before k8s-setup.sh

# change to this directory
#cd "$(dirname "${BASH_SOURCE[0]}")" || exit

echo "$(date +'%Y-%m-%d %H:%M:%S')  pre-k3d start" >> "$HOME/status"

if ! docker network ls | grep k3d; then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  creating registry" >> "$HOME/status"
  # create local registry
  docker network create k3d

  # create container registry
  k3d registry create registry.localhost --port 5500
  docker network connect k3d k3d-registry.localhost
fi

# add the host name to /etc/hosts
{
  echo "" | sudo tee -a /etc/hosts
  echo "127.0.0.1 $(hostname)"
} | sudo tee -a /etc/hosts

echo "$(date +'%Y-%m-%d %H:%M:%S')  pre-k3d complete" >> "$HOME/status"
