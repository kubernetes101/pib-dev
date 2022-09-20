# PiB Outer-Loop Multi-Cluster

## Create a unique cluster identifier

    ```bash

    export MY_BRANCH=$(echo $GITHUB_USER | tr '[:upper:]' '[:lower:]')

    ```

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

## Create 3 Clusters

- Use one Azure Resource Group
- Create a cluster in each region
  - You can use different names as long as they are unique
  - Standard Naming Format
    - region (central, east, west)
    - state
    - city
    - store_number

    ```bash

    flt create \
        -g $MY_BRANCH-fleet \
        -c central-tx-$MY_BRANCH-1001 \
        -c east-ga-$MY_BRANCH-1001 \
        -c west-wa-$MY_BRANCH-1001

    ```

## Verifying the Clusters

- Follow the instructions in the [outer-loop lab](./outer-loop.md)
- Return here once the clusters are verified

## IMDb Deployment

- By default, the IMDb app is only deployed to the central region cluster
- Experiment with different deployments
  - Make sure you're in the apps/imdb directory

  ```bash

  # deploy to central and west regions
  flt targets add region:west
  flt targets deploy

  # deploy to just the east region
  flt targets clear
  flt targets add region:east
  flt targets deploy

  # deploy to all clusters
  flt targets clear
  flt targets add all
  flt targets deploy

  ```

## Deploy Dogs-Cats App

- Dogs and cats app is a simple "voting" app for demo purposes
  - Note that dogs-cats and IMDb cannot be deployed to the same cluster due to ingress conflicts
    - In a production environment, you would add ingress rules for host, url, or port based routing

> Start in the apps/imdb directory

```bash

# start in the apps/imdb directory
flt targets clear

# deploy IMDb to the central region
flt targets add region:central

cd ../dogs-cats

# deploy dogs-cats to the west region
flt targets clear
flt targets add region:west
flt targets deploy

# wait for ci-cd to complete
flt check app imdb
flt check app dogs
flt curl /version

```
