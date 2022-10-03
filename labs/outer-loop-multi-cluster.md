# PiB Outer-Loop Multi-Cluster

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

    # start in the base of the repo
    cd $PIB_BASE

    flt create \
        -g $MY_BRANCH-fleet \
        -c central-tx-$MY_BRANCH-1001 \
        -c east-ga-$MY_BRANCH-1001 \
        -c west-wa-$MY_BRANCH-1001

    ```

## Verifying the Clusters

- Update Git Repo after [CI-CD](https://github.com/kubernetes101/pib-dev/actions) is complete (usually about 30 seconds)

  ```bash

  # update the git repo after ci-cd completes
  git pull

  # add ips to repo
  git add ips
  git commit -am "added ips"
  git push

  ```

- Verify clusters setup

  ```bash

  # check the setup for "complete"
  # rerun as necessary
  flt check setup

  ```

- Check Heartbeat

  ```bash

  # check that heartbeat is running on your cluster
  flt check heartbeat

  # check heartbeat on clusters in specific region
  flt check heartbeat --filter central

  ```

## IMDb Deployment

- By default, the IMDb app is not deployed to any clusters
- Experiment with different deployments

  ```bash

  # start in the apps/imdb directory
  cd $PIB_BASE/apps/imdb

  # deploy to central and west regions
  flt targets add region:central region:west
  flt targets deploy

  # wait for ci-cd to complete and update the cluster
  git pull
  flt sync

  # check the cluster for imdb
  flt check app imdb

  # deploy to just the east region
  flt targets clear
  flt targets add region:east
  flt targets deploy

  # wait for ci-cd to complete and update the repo
  git pull
  flt sync

  # check the cluster for imdb
  flt check app imdb

  # deploy to all clusters
  flt targets clear
  flt targets add all
  flt targets deploy

  # wait for ci-cd to complete and update the repo
  git pull
  flt sync

  # check the cluster for imdb
  flt check app imdb

  ```

## Deploy Dogs-Cats App

- Dogs and cats app is a simple "voting" app for demo purposes
  - Note that dogs-cats and IMDb cannot be deployed to the same cluster due to ingress conflicts
    - In a production environment, you would add ingress rules for host, url, or port based routing

> Start in the apps/imdb directory

```bash

# start in the apps/imdb directory
cd $PIB_BASE/apps/imdb

# deploy IMDb to the central region
flt targets clear
flt targets add region:central

cd ../dogs-cats

# deploy dogs-cats to the west region
flt targets clear
flt targets add region:west
flt targets deploy

# wait for ci-cd to complete and update the repo
git pull
flt sync

# check apps on cluster
flt check app imdb
flt check app dogs
flt curl /version

```

## Clean Up

- Once you are finished with the workshop, you can delete your Azure resources

```bash

# start in the base of the repo
cd $PIB_BASE
git pull

# delete the Azure resources
flt delete central-tx-$MY_BRANCH-1001
flt delete east-ga-$MY_BRANCH-1001
flt delete west-wa-$MY_BRANCH-1001
flt delete $MY_BRANCH-fleet

# remove ips file
rm -f ips

# reset the targets
cd apps/imdb
flt targets clear
cd ../dogs-cats
flt targets clear
cd ../..

# update the repo
git commit -am "deleted fleet"
git push

```
