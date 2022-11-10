# PiB outer-loop to AKS-IoT

## AKS-IoT is in Preview

> AKS-IoT is in preview so there's a chance these instructions will
change over time!

Reach out to the [soldevx](mailto:soldevx@microsoft.com) team for access to AKS-IoT preview.
TODO: Is this how they would want to be contacted?

## PLEASE NOTE: This document is a work in progress

The current instructions are run in PowerShell.

## AKS-IoT Setup

- Create a new Azure VM with Windows 10
  - You can also use your local Windows 10 or Windows 11 computer
- Run Windows Update
- Install Hyper-V - **NOTE**: This requires a reboot!
- Install, if not already on your machine:
  - [git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)
  - [GitHub CLI](https://cli.github.com/manual/installation)
  - [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
  - [VS Code](https://code.visualstudio.com/Download)

- Install Chocolatey

  ```powershell

  Set-ExecutionPolicy Bypass -Scope Process -Force; `
    [System.Net.ServicePointManager]::SecurityProtocol = `
    [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; `
    iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))`

  ```

- Install Helm
  - `choco install kubernetes-helm`

## Update `bootstrap/aksiot-userconfig.json`

TODO: Does this file exist yet? or is it created in a different process?

From the Azure portal, add these values to the file:

- ResourceGroupName
- ClusterName

## Set permanent Env Vars

Update with your environment to include your values:

```powershell

setx AZ_TENANT yourTenant
setx AZ_SP_ID yourServicePrincipal
setx AZ_SP_KEY yourSPKey
setx PAT yourPAT
setx PIB_PAT %PAT%
setx GITHUB_TOKEN %PAT%

setx PIB_CLUSTER your-cluster-name-101
setx PIB_RESOURCE_GROUP yourRG
setx PIB_FULL_REPO https://github.com/yourOrg/yourRepo
setx PIB_BRANCH yourBranch

```

> You will need to exit and start a new shell after running the `setx` commands.

## Install AKS-IoT

- TODO: update with download instructions

```powershell

# when prompted for git credentials, use your PAT to avoid 2FA setup / issues

# start in the directory you copied the file share to
git clone https://github.com/kubernetes101/pib-dev

# install the msi
cd bin
AksIot-K3s.msi
cd ..

```

- Set PiB Base to current directory

```powershell

cd pilot-in-a-box

cd

setx PIB_BASE <value from cd without CRLF>

```

- From AKS-IoT/bootstrap directory
- These commands must be run from the AKS-IoT Powershell

```powershell

# start elevated shell
# (optional) create a shortcut on your desktop
LaunchPrompt.cmd

# add az cli extensions
az extension add --upgrade --name connectedk8s
az extension add --upgrade --name k8s-configuration
az extension add --upgrade --name k8s-extension
az extension add --upgrade --name k8s-configuration

az provider register --namespace Microsoft.Kubernetes
az provider register --namespace Microsoft.KubernetesConfiguration
az provider register --namespace Microsoft.ExtendedLocation

# Initialize Arc
Initialize-ArcIot

# update help (this is required later in the process)
update-help

```

## Create a K3s Cluster

```powershell

# create a single machine cluster
# choose 1 (or edit)

New-AksIotDeployment -SingleMachineCluster -WorkloadType Linux -ServiceIPRangeSize 10 -LinuxVmCpuCount 2 -LinuxVmMemoryInMB 4096

New-AksIotDeployment -SingleMachineCluster -WorkloadType Linux -ServiceIPRangeSize 10 -LinuxVmCpuCount 4 -LinuxVmMemoryInMB 8192

New-AksIotDeployment -SingleMachineCluster -WorkloadType Linux -ServiceIPRangeSize 10 -LinuxVmCpuCount 6 -LinuxVmMemoryInMB 12288

# check cluster
kubectl get nodes
kubectl get pods -A

# store the token
del servicetoken.txt
$secret = kubectl get serviceaccount aksiot-admin-user -o jsonpath='{$.secrets[0].name}'
kubectl get secret $secret -o jsonpath='{$.data.token}' > token.txt
certutil -decode token.txt servicetoken.txt
del token.txt

```

## Arc Enable the Cluster

> Make sure you set your Env Vars above and have started a new shell.

```powershell

# (optional) login to Azure with SP
az login --service-principal --tenant $Env:AZ_TENANT --username $Env:AZ_SP_ID --password $Env:AZ_SP_KEY

# connect the cluster to Arc
az connectedk8s connect --name $Env:PIB_CLUSTER --resource-group $Env:PIB_RESOURCE_GROUP

```

## Arc Enabled GitOps

Create GitOps config based on the following: `pilot-in-a-box/labs/advanced-labs/aks-iot/sample-cluster.txt`
will become `pilot-in-a-box/clusters/your-cluster.yaml`. Then git add, commit, push. Once the
`ci-cd` GitHub action is complete, you can enable Arc.

```powershell

az k8s-configuration flux create `
  --cluster-type connectedClusters `
  --interval 1m `
  --kind git `
  --name gitops `
  --namespace flux-system `
  --scope cluster `
  --timeout 3m `
  --https-user gitops `
  --cluster-name $Env:PIB_CLUSTER `
  --resource-group $Env:PIB_RESOURCE_GROUP `
  --url $Env:PIB_FULL_REPO `
  --branch $Env:PIB_BRANCH `
  --https-key $Env:PIB_PAT `
  --kustomization `
      name=flux-system `
      path=./clusters/$Env:PIB_CLUSTER/flux-system/listeners `
      timeout=3m `
      sync_interval=1m `
      retry_interval=1m `
      prune=true `
      force=true

```

## Test Arc and Arc GitOps

Open the Azure Portal and open the Arc Blade. Locate your cluster and get a Service Token.
TODO: Is this accurate?--> From `AKS-IoT\bootstrap` directory, copy the Service Token into a file
`servicetoken.txt`.

## Delete Cluster

From within the the `aks-iot/bootstrap` directory:

```powershell

LaunchPrompt.cmd

# delete the cluster
Remove-AksIotNode

exit

```
