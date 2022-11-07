# PiB outer-loop with Arc Enabled Gitops

## Introduction

- This lab extends the previous outer-loop labs ([outer-loop](./outer-loop.md) and [multi-cluster](./outer-loop-multi-cluster.md)) by Arc enabling the clusters in the fleet
- Arc enablement allows the k3d clusters in the fleet to be monitored and managed from the Azure Portal
- In this lab we will:
  - Create an Arc enabled, single-cluster fleet
  - Deploy an application to the fleet (with GitOps)
  - Validate the deployment in the Azure Arc Portal

## Validate Cluster Identifier and Working Branch

```bash

# by default, MY_BRANCH is set to your lower case GitHub User Name
# the variable is used to uniquely name your clusters
# the value can be overwritten if needed
echo $MY_BRANCH

# make sure your branch is set and pushed remotely
# commands will fail if you are in main branch
git branch --show-current

```

## Login to Azure

- Login to Azure using `az login --use-device-code`
  > Use `az login --use-device-code --tenant <tenant>` to specify a different tenant
  - If you have more than one Azure subscription, select the correct subscription

    ```bash

    # verify your account
    az account show

    # list your Azure accounts
    az account list -o table

    # set your Azure subscription
    az account set -s mySubNameOrId

    # verify your account
    az account show

    ```

- Validate user role on subscription
  > Make sure your RoleDefinitionName is `Contributor` or `Owner` to create resources in this lab succssfully

  ```bash

  # get az user name and validate your role assignment
  principal_name=$(az account show --query "user.name" --output tsv | sed -r 's/@.*//')
  az role assignment list --query "[].{principalName:principalName, roleDefinitionName:roleDefinitionName, scope:scope} | [? contains(principalName,'$principal_name')]" -o table

  ```

## Create/Set Managed Identity

- If you don't already have managed identity set in your subscription, follow [these steps](./azure-codespaces-setup.md#create-resource-group) to create RG and MI
- Run `flt env` and make sure `PIB_MI` is set

## Create an Arc Enabled Dev Cluster

```bash

# set MY_CLUSTER
export MY_CLUSTER=central-tx-atx-$MY_BRANCH

# create an arc enabled cluster
# it will take about 2 minutes to create the VM
flt create cluster -c $MY_CLUSTER --arc

```

### `flt create` Arc Enablement Details

- Running `flt create` with the --arc flag affects what is run in the vm setup scripts by setting the `PIB_ARC_ENABLED` environment variable to true
- The key differences are in:
  - [flux-setup.sh](../vm/setup/flux-setup.sh)
    - This script is ignored as Arc enabled flux is used instead
  - [arc-setup.sh](../vm/setup/arc-setup.sh)
    - This script adds Arc dependencies, connects the cluster to Arc, and configures Arc enabled flux
    - Run `code /workspaces/pib-dev/vm/setup/arc-setup.sh` to view the commands used for configuration

## Update Git Repo

- [CI-CD](https://github.com/kubernetes101/pib-dev/actions) generates the deployment manifests
  - Wait for CI-CD to complete (usually about 30 seconds)

```bash

# update the git repo after ci-cd completes
git pull

# add ips to repo
git add ips
git commit -am "added ips"
git push

```

## Verify the Cluster Setup

```bash

# check the setup for "complete"
# rerun as necessary
flt check setup

# optional - use the Linux watch command
# press ctl-c after "complete"
watch flt check setup

```

## Deploy IMDb App

- Deploy IMDb app to Arc enabled K3d cluster

  ```bash

  # start in the apps/imdb directory
  cd $PIB_BASE/apps/imdb

  # deploy to central and west regions
  flt targets add all
  flt targets deploy

  ```

- Wait for [ci-cd](https://github.com/kubernetes101/pib-dev/actions) to finish
- Force cluster to sync

  ```bash

  # should see imdb added
  git pull

  # force flux to reconcile
  flt sync

  ```

## Validate IMDb app on Azure Arc

- Get Azure Arc bearer token by running
  `flt az arc-token`
- Login to Azure Portal and navigate to `Azure Arc` service
- Click on `Kubernetes clusters` from the left nav and select your cluster
- Click on `Workloads` from the left nav and place bearer token retrieved earlier
- Validate the IMDb app running on the cluster

## Delete Your Cluster

- Once you're finished with the workshop and experimenting, delete your cluster

  ```bash

  # start in the root of your repo
  cd $PIB_BASE
  git pull

  # delete azure resource
  flt delete $MY_CLUSTER

  # remove ips file
  rm ips

  # reset the targets
  cd apps/imdb
  flt targets clear
  cd ../..

  # update the repo
  git commit -am "deleted cluster"
  git push

  ```
