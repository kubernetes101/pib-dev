# PiB outer-loop to AKS on Azure

## Create a unique cluster identifier

```bash

export MY_BRANCH=$(echo $GITHUB_USER | tr '[:upper:]' '[:lower:]')

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

## Create Arc enabled AKS Cluster

### Create AKS Cluster

> This AKS setup is insecure and intended for learning, dev and test only. For secure or production clusters, please refer [AKS Secure Baseline](https://github.com/mspnp/aks-baseline)

  ```bash

  # set MY_AKS_CLUSTER
  export MY_AKS_CLUSTER=central-tx-atx-$MY_BRANCH-aks

  # set MY_RG
  export MY_RG=$MY_BRANCH-rg

  ```

  ```bash

  # create resource group
  az group create --name $MY_RG --location eastus

  # create AKS cluster
  # this may take 3-5 mins
  az aks create -g $MY_RG -n $MY_AKS_CLUSTER --enable-managed-identity --node-count 1 --generate-ssh-keys

  ```

### Arc enable the AKS Cluster

   ```bash

   # install the connectedk8s Azure CLI extension
   az extension add --name connectedk8s

   # get aks creds
   az aks get-credentials --resource-group $MY_RG --name $MY_AKS_CLUSTER

   # arc enable
   # this may take 2-3 mins
   az connectedk8s connect --name $MY_AKS_CLUSTER --resource-group $MY_RG --location eastus

   ```

## Set up for GitOps

### Flt setup

  ```bash

  # create the cluster metadata
  flt create --gitops-only -c $MY_AKS_CLUSTER

  ```

- This will create the cluster metadata for GitOps
- Wait for ci-cd to complete
- run `git pull`
  - You should see the yaml files created in the `clusters` directory

### Create flux secret

  ```bash

  # create secret for GitOps
  kubectl apply -f "clusters/$MY_AKS_CLUSTER/flux-system/namespace.yaml"
  flux create secret git flux-system -n flux-system --url "$PIB_FULL_REPO" -u gitops -p "$PIB_PAT"

  ```

## Create GitOps configuration with Arc

- All of the values you need for Arc setup are displayed via `flt env` and starts with PIB_*

- From the Azure Arc Portal
  - Select `Kubernetes Clusters` in left nav
  - Select the cluster
  - Select `GitOps` in the left nav
  - Click `Create`
  - Basics:
    - Configuration Name: `gitops`
    - Namespace: `flux-system`
    - Scope: `cluster`
  - Source:
    - Source kind: `Git Repository`
    - Repository URL: `https://github.com/kubernetes101/pib-dev`
    - Reference type: `Branch`
    - Branch: value of MY_BRANCH
    - Repository Type:`Private`
    - Authentication source: `Provide Authentication information here`
    - HTTPS User: `gitops`
    - HTTPS Key: value of PIB_PAT from `flt env`
    - HTTPS CA Cert: leave blank
    - Sync interval: 1 minute
    - Sync timeout: 3 minutes
  - Kustomization
    - Instance name: flux-system
    - Path: ./clusters/{{cluster-name}}/flux-system
    - Sync interval: 1 minute
    - Sync timeout: 3 minutes
    - Retry interval: 1 minute
    - Prune: checked
    - Force: checked
    - Depends on: leave blank
  - Wait for gitops configuration to be created
  - Select `Configuration objects` from the left nav of `GitOps` config
  - Wait for `gitops-flux-system` to be created
    - Wait for it to be `Compliant`

## Check in Arc Portal

- Use the Arc Portal to check GitOps setup
  - Check cluster namespaces and workloads

## Deploy sample app

```bash

# make sure you're in the apps/imdb directory
cd apps/imdb

# check deployment targets
# should be []
flt targets list

# deploy to all clusters
flt targets clear
flt targets add all
flt targets deploy

## wait for ci-cd to run
git pull

# check the clusters in Arc for the imdb workload

```

## Cleanup

- Once you're finished with the workshop and experimenting, delete your cluster and Azure resources

```bash

# delete rg and AKS cluster
az group delete -n $MY_RG

# start in the base of the repo
cd $PIB_BASE
git pull

# clear cluster metadata
flt delete $MY_AKS_CLUSTER

# reset the targets
cd apps/imdb
flt targets clear
cd ../..

# update the repo
git commit -am "deleted fleet"
git push

```
