# Azure Subscription and Codespaces Setup

- We use Azure Managed Identity and Codespaces Secrets for credentials

> Work in Progress

## Login to Azure

- Login to Azure using `az login --use-device-code`
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

## Setup

- In order to use Azure Arc, HTTPS, or DNS, you must configure your Azure subscription and Codespaces Secrets

## Shared Personal Access Token

> Codespaces PATs expire after 8 hours
>
> Create a long-lived PAT

- Create a shared GitHub Personal Access Token
  - Grant Repos and Packages permission
  - Grant SSO permission as needed
  - You can use an existing PAT with proper permissions
- Create a Codespaces Secret for the GitHub PAT

```bash

gh secret set PIB_PAT --body "YourSharedPAT"

# list secrets
gh secret list

```

## Create Resource Group

- We use `tld` for our resource group
  - The RG may contain
    - Managed Identity
    - Platform Key Vault
    - DNS Service

```bash

# change if desired
export rg=tld
az group create -g $rg -l westus3

# add RG secret
gh secret set PIB_DNS_RG --body $rg

# list secrets
gh secret list

```

### Create Managed identity

- Required for Azure access from the dev/test clusters

```bash

# Managed Identity name
export mi=pib_mi

# create MI and add CS secret
gh secret set PIB_MI --body $(az identity create --name $mi --resource-group $rg --query id -o tsv)

# list secrets
gh secret list

```

## Create Shared SSH Key

- This will allow multiple users to access the clusters from the same branch
  - The flt CLI uses SSH to connect to the dev/test clusters
- `.devcontainer/post-create.sh` will decrypt and save the SSH from Codespaces Secrets when a new Codespace is created

```bash

# create (or copy) SSH key
# do not overwrite existing key
# leave passphrase blank
ssh-keygen -t ecdsa -b 521 -f $HOME/.ssh/id_rsa

# add ssh key to Codespaces Secrets
gh secret set ID_RSA --body $(cat $HOME/.ssh/id_rsa | base64 | tr -d '\n')
gh secret set ID_RSA_PUB --body $(cat $HOME/.ssh/id_rsa.pub | base64 | tr -d '\n')

# list GitHub Secrets
gh secret list

```

## Create Azure Key Vault

- Create Azure Key Vault from the Azure Portal
- Grant Managed Identity permissions to the Key Vault

```bash

# change to your key vault name
export kv=pib_kv

# set Key Vault secret
gh secret set PIB_KEYVAULT --body $kv

# list secrets
gh secret list

```

## Create DNS Zone

- required for HTTPS
- Purchase a domain from the Azure Portal (or bring your own)
- Create a DNS Zone using PIB_DNS_RG from above
- Grant the Managed Identity access to the DNS Zone

```bash

# change to your domain
export ssl=cseretail.com

# add SSL secret
gh secret set PIB_SSL --body $ssl

# list secrets
gh secret list

```

## Create Service Principal

- optional
- allows login with `flt az login` using the SP credentials
- Grant SP access to Key Vault if setup

```bash

# create SP
id=$(az ad sp create-for-rbac \
        --name pib_sp \
        --role owner \
        --scopes /subscriptions/$(az account show --output tsv --query id) \
        --output tsv \
        --query appId)

key=$(az ad sp create-for-rbac \
        --name pib_sp \
        --role owner \
        --scopes /subscriptions/$(az account show --output tsv --query id) \
        --output tsv \
        --query password)

# add Azure SP login secrets
gh secret set AZ_TENANT --body $(az account show  --output tsv --query tenantId)
gh secret set AZ_SP_ID --body $id
gh secret set AZ_SP_KEY --body $key

# list secrets
gh secret list

```
