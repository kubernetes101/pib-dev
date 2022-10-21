#!/bin/bash

### to run manually
# cd $HOME
# pib/vm/setup/setup.sh

# this is the main VM setup script

# env variables defined in /etc/bash.bashrc
    # PIB_ARC_ENABLED
    # PIB_BRANCH
    # PIB_CLUSTER
    # PIB_NO_SETUP
    # PIB_DNS_RG
    # PIB_FQDN
    # PIB_ME
    # PIB_REPO
    # PIB_RESOURCE_GROUP
    # PIB_SSL

# change to this directory
dir="$(dirname "${BASH_SOURCE[0]}")" || exit

echo "$(date +'%Y-%m-%d %H:%M:%S')  setup start" >> "$HOME/status"

# can't continue without install.sh
if [ ! -f "$dir"/install.sh ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  install.sh not found" >> "$HOME/status"
  echo "install.sh not found"
  exit 1
fi

# can't continue without azure.sh
if [ ! -f "$dir/azure.sh" ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  azure.sh not found" >> "$HOME/status"
  echo "azure.sh not found"
  exit 1
fi

# can't continue without dns.sh
if [ ! -f "$dir"/dns.sh ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  dns.sh not found" >> "$HOME/status"
  echo "dns.sh not found"
  exit 1
fi

# can't continue without k8s-setup.sh
if [ ! -f "$dir"/k8s-setup.sh ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  k8s-setup.sh not found" >> "$HOME/status"
  echo "k8s-setup.sh not found"
  exit 1
fi

# can't continue without flux-setup.sh
if [ ! -f "$dir"/flux-setup.sh ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  flux-setup.sh not found" >> "$HOME/status"
  echo "flux-setup.sh not found"
  exit 1
fi

set -e

# run setup scripts
"$dir"/install.sh

# don't run setup steps in no-setup mode
# this allows for step-by-step setup of the rest of the steps, if desired
if [ "$PIB_NO_SETUP" = "true" ]
then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  no-setup mode" >> "$HOME/status"
  echo "no-setup mode"
  exit 0
fi

# run setup scripts
"$dir"/azure.sh
"$dir"/dns.sh

# run pre-k8s.sh
if [ -f "$dir"/pre-k8s.sh ]
then
  # run as PIB_ME
  "$dir"/pre-k8s.sh
else
  echo "$(date +'%Y-%m-%d %H:%M:%S')  pre-k8s.sh not found" >> "$HOME/status"
fi

# run k8s-setup
"$dir"/k8s-setup.sh

# make sure the repo is up to date
git -C "$dir" pull

# run pre-flux.sh
if [ -f "$dir"/pre-flux.sh ]
then
  "$dir"/pre-flux.sh
else
  echo "$(date +'%Y-%m-%d %H:%M:%S')  pre-flux.sh not found" >> "$HOME/status"
fi

# setup flux
"$dir"/flux-setup.sh

# run pre-arc.sh
if [ -f "$dir"/pre-arc.sh ]
then
  "$dir"/pre-arc.sh
else
  echo "$(date +'%Y-%m-%d %H:%M:%S')  pre-arc.sh not found" >> "$HOME/status"
fi

# setup azure arc
if [ -f "$dir"/arc-setup.sh ]
then
  "$dir"/arc-setup.sh
else
  echo "$(date +'%Y-%m-%d %H:%M:%S')  arc-setup.sh not found" >> "$HOME/status"
fi

# run post.sh
if [ -f "$dir"/post-setup.sh ]
then
  "$dir"/post-setup.sh
else
  echo "$(date +'%Y-%m-%d %H:%M:%S')  post-setup.sh not found" >> "$HOME/status"
fi

echo "$(date +'%Y-%m-%d %H:%M:%S')  setup complete" >> "$HOME/status"
echo "complete" >> "$HOME/status"
