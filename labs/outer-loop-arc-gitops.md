# PiB outer-loop with Arc Enabled Gitops

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

## Create/Set Managed Identity

- If you don't already have managed identity set in your subscription, follow [these steps](./azure-codespaces-setup.md#create-managed-identity) to create RG and MI
- Run `flt env` and make sure `PIB_MI` is set

## Create an Arc enabled Dev Cluster

```bash

# set MY_CLUSTER
export MY_CLUSTER=central-tx-atx-$MY_BRANCH

# create an arc enabled cluster
# it will take about 2 minutes to create the VM
flt create cluster -c $MY_CLUSTER --arc

```

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
