# PiB Outer-Loop Multi-Cluster

## Validate cluster identifier and working branch

```bash

# by default, MY_BRANCH is set to your lower case GitHub User Name
# the variable is used to uniquely name your clusters
# the value can be overwritten if needed
echo $MY_BRANCH

```

## Login to Azure

Login to Azure using `az login --use-device-code`.

Use `az login --use-device-code --tenant <tenant>` to specify a different tenant if you have access
to more than one tenant.

If you have more than one Azure subscription, select the correct subscription:

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

Validate the user role on subscription. Make sure your RoleDefinitionName is `Contributor` or `Owner`
to create resources in this lab successfully.

  ```bash

  # get az user name and validate your role assignment
  principal_name=$(az account show --query "user.name" --output tsv | sed -r 's/[@]+/_/g')
  az role assignment list --query "[].{principalName:principalName, roleDefinitionName:roleDefinitionName, scope:scope} | [? contains(principalName,'$principal_name')]" -o table

  ```

## Create 3 Clusters

In one Azure Resource Group, create a cluster in three different regions. You can use different
names as long as they are unique.

In our example we're using the following name format:
region (central, east, west), state, the branch name, and a "store number."

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

Update Git Repo after [CI-CD](https://github.com/kubernetes101/pib-dev/actions) is complete.
This usually takes about 30 seconds.

```bash

# update the git repo after ci-cd completes
git pull

# add ips to repo
git add ips
git commit -am "added ips"
git push

```

Then verify the cluster's setup with `flt check setup`. Check the setup status for "complete",
and rerun as necessary.

You can check the heartbeat.

```bash

# check that heartbeat is running on your cluster
flt check heartbeat

# check heartbeat on clusters in specific region
flt check heartbeat --filter central

```

## IMDb Deployment

By default, the IMDb app is not deployed to any clusters. So now we can experiment with
different deployments!

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

The "Dogs and Cats" app is a simple "voting" app for demo purposes.

> Note here that dogs-cats and IMDb cannot be deployed to the same cluster due to ingress conflicts.

In a production environment, you would add ingress rules for host, url, or port-based routing.

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

Once you are finished with the workshop, you can delete your Azure resources.

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
