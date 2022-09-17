# PiB outer-loop: Vision on Edge (VoE)

## Introduction

- VoE is an open-source tool that builds vision-based intelligent edge solutions using Machine Learning.  Visit this [repo](https://github.com/Azure-Samples/azure-intelligent-edge-patterns/tree/master/factory-ai-vision) for more information.
- The VoE app is being deprecated. It is used for demonstrative purposes to show how to deploy a more complex app with PiB.

## Fleet Configuration Prerequisites

- There are a few dependencies and prerequisites that must be configured before deploying the VoE app.
- Azure Resources
  - Azure IoT Hub
  - Azure Cognitive Services
- Fleet VM Configuration
  - At least 8 cores
  - Kubernetes secrets

### Create Azure Resources

#### Define resource name variables

```bash

export VOE_HUB_NAME=voe-hub-$MY_BRANCH

export VOE_RG=voe-rg-$MY_BRANCH

export VOE_AZ_COG_SVC_NAME=voe-acs-$MY_BRANCH

```

#### Login to Azure CLI

```bash

az login --use-device-code

```

#### Create Azure IoT Hub

```bash

# add azure-iot extension
az extension add -n azure-iot

az iot hub create --resource-group $VOE_RG --name $VOE_HUB_NAME

```

#### Create Azure Cognitive Services

```bash

# you may have to create a cognitive services multi-service account in the azure portal to fulfill the requirement to agree to the responsible AI terms for the resource
az cognitiveservices account create --kind CognitiveServices --name $VOE_AZ_COG_SVC_NAME --resource-group $VOE_RG --sku S0 --location yourlocation

```

### Update fleet creation script

- Add the following lines to vm/setup/pre-flux.sh and replace the values in [] with the names of the resources created.
  - This will run on the fleet vm during setup and will create the voe namespace and required k8s secret.
  - The VM uses Managed Identity to retrieve the connection string values from the IoT Hub.

- Do NOT run this fence!

```bash

# add the iot extension
az extension add -n azure-iot

# azure iot secrets
echo "IOTHUB_CONNECTION_STRING=$(az iot hub connection-string show --hub-name [your-voe-hub-name] -o tsv)" > "$HOME/.ssh/iot.env"
echo "IOTEDGE_DEVICE_CONNECTION_STRING=$(az iot hub device-identity connection-string show --hub-name [your-voe-hub-name] --device-id "$(hostname)" -o tsv)" >> "$HOME/.ssh/iot.env"

# azure cognitive services secrets
echo "ENDPOINT=$(az cognitiveservices account show -n [your-voe-acs-name] -g [your-voe-rg] --query properties.endpoint -o tsv)" > "$HOME/.ssh/acs.env"
echo "TRAINING_KEY=$(az cognitiveservices account keys list -n [your-voe-acs-name] -g [your-voe-rg] --query key1 -o tsv)" >> "$HOME/.ssh/acs.env"

kubectl create ns voe

kubectl create secret generic azure-env --from-env-file "$HOME/.ssh/iot.env" --from-env-file="$HOME/.ssh/acs.env" -n voe

```

- After updating the script, push your changes

```bash

git add .
git commit -m "updated pre-flux.sh"
git push

```

## Create a fleet

- The VoE application requires at least 8 cores, this must be specified at fleet creation with the --cores flag.

```bash

# before creating the cluster, make sure PIB_MI is set
# Managed Identity (MI) is required for the voe k8s secrets to be created properly
flt env PIB_MI

# set MY_CLUSTER
export MY_CLUSTER=central-tx-voe-$MY_BRANCH

# create your cluster
flt create -c $MY_CLUSTER --cores 8 --verbose

```

### Add iot devices (clusters in fleet) to IoT Hub

You must run this command for each cluster in the fleet

```bash

az iot hub device-identity create --ee -n $VOE_HUB_NAME -d $MY_CLUSTER

```

## Create apps/voe directory

- Add voe to the apps/ directory

```bash

cd advanced-scenarios

cp -aR voe ../apps

# commit and push changes
git add ../
git commit -m "Adding voe to apps/"
git push

```

## Deploy the VoE app to your fleet

```bash

# start in the apps/voe directory
cd ../apps/voe

# make sure repo is up to date
git pull

# add all clusters as a target
flt targets add all

# other options
# flt targets add region:central
# flt targets add zone:central-tx
# flt targets add district:central-tx-voe

# deploy voe via ci-cd and GitOps Automation
flt targets deploy

```

## Verify VoE deployment

```bash

# check the cluster for voe
flt check app voe

# check the status of the pods running on the cluster
# it can take ~2 minutes for all of the voe services to be ready
flt exec kic pods

# curl healthz and readyz endpoints
flt curl /healthz
flt curl /readyz

```

## Navigate to VoE in the browser

- Get the FQDN of your cluster
- Copy and paste the FQDN into your browser
  - You should get the VoE home page

    ```bash

    # display the FQDN
    echo $MY_CLUSTER.$PIB_SSL

    ```

## Delete Your Cluster

- Once you're finished with the workshop and experimenting, delete your cluster

```bash

# start in the root of your repo
cd $PIB_BASE
git pull
flt delete $MY_CLUSTER
rm ips
git commit -am "deleted cluster"
git push

```
