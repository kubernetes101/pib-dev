# PiB Automated Canary Deployment Using Flagger

## Introduction

[Flagger](https://flagger.app/) is a progressive delivery tool that automates the release process
for applications running on Kubernetes. It takes a Kubernetes deployment and creates a series of objects
(Kubernetes deployments, ClusterIP services and Contour HTTPProxy) for an application. These objects
expose the application in the cluster and drive the canary analysis and promotion.

## Lab Prerequisites

Complete outer-loop [Lab 1](../../outer-loop.md) **BUT** skip the [Delete Your Cluster](../../outer-loop.md#delete-your-cluster)
section. The subsequent sections rely on the environment variables set in these labs.

## Validate cluster identifier and working branch

```bash

# make sure MY_CLUSTER is set from previous lab
echo $MY_CLUSTER

# make sure your branch is set and pushed remotely
# commands will fail if you are in main branch
git branch --show-current

```

## Install Flagger

```bash

# make sure you are in canary directory
cd $PIB_BASE/labs/advanced-labs/canary

# copy flagger to apps directory
cp -R ./flagger ../../../apps

# add and commit the flagger app
cd $PIB_BASE
git add .
git commit -am "added flagger app"
git push

cd apps/flagger

# check deploy targets (should be [])
flt targets list

# if not [] then clear the targets
# flt targets clear

# add all clusters as a target
flt targets add all

# deploy the changes
flt targets deploy

```

### Check GitHub Action Status

Check that your GitHub Action is running, either queued or in-progress. Check the action here:
<https://github.com/kubernetes101/pib-dev/actions> TODO: Is this URL for all builds or will it be for
a presumably forked repo?

### Check Deployment

Once the action completes successfully, check the deployment:

```bash

# you should see flagger added to your cluster
git pull

# force flux to sync
# flux will sync on a schedule - this command forces it to sync now for debugging
flt sync

# check that flagger is deployed to your cluster
# NOTE: We also deploy prometheus to scrape metrics to monitor Canary deployment
flt check app flagger
flt check app prometheus

```

## Update Reference App to Use Canary Deployment Strategy

In order to update IMDb reference app to use canary deployment template, you'll need to update a few
values. In `apps/imdb/app.yaml`, change the `template` line to be `template: pib-service-canary` instead
of `template: pib-service`

```bash

# deploy imdb with canary template
cd ../imdb
flt targets deploy

```

Once the [GitHub action](https://github.com/kubernetes101/pib-dev/actions) is completed, force flux
to sync:

```bash

# force flux to sync
# flux will sync on a schedule - this command forces it to sync now for debugging
git pull
flt sync

```

The reference app should be updated with Canary Deployment objects listed:

```bash

deployment.apps/imdb
deployment.apps/imdb-primary
deployment.apps/webv-imdb
service/imdb
service/imdb-canary
service/imdb-primary
service/webv-imdb
httpproxy.projectcontour.io/imdb

```

Then validate primary and canary objects in the cluster:

```bash

flt ssh $MY_CLUSTER
kic pods
kic svc
kubectl get canary -n imdb

# exit from cluster
exit

```

## Observe Automated Canary Promotion

Next, trigger a canary deployment by updating the container image for IMDb. In `apps/imdb/app.yaml`,
change the image tag from `latest` to `beta`: `image: ghcr.io/cse-labs/pib-imdb:beta`.

```bash

# deploy imdb with updated version
cd ../imdb
flt targets deploy

```

Once the [GitHub action](https://github.com/kubernetes101/pib-dev/actions) is completed, force flux
to sync again:

```bash

# force flux to sync
# flux will sync on a schedule - this command forces it to sync now for debugging
git pull
flt sync

```

You can then observe the canary promotion in K9s:

```bash

# start K9s for the cluster
flt ssh $MY_CLUSTER
k9s <enter>

```

In K9s, you can type a few commands to view and observe the objects.

- Type `:canaries <enter>` to view canary object
- Observe `status` and `weight` for canary promotion

> Flagger detects the deployment version change and starts a new rollout with 20% traffic
> progression. Once a canary `status` is updated to `Succeeded`, 100% of the traffic should be routed
> to the new version.

To go back:

- Press `enter` again and scroll to bottom to see events
- Press `escape` to go back
- Exit K9s: `:q <enter>`
- Exit from cluster: `exit <enter>`

## Monitoring Canary Deployments Using Grafana

Flagger comes with a Grafana dashboard made for canary analysis. Install Grafana following the instructions
below.

```bash

# cd to canary directory
cd $PIB_BASE/labs/advanced-labs/canary

# copy flagger to apps directory
cp -R ./flagger-grafana ../../../apps

# add and commit the flagger-grafana app
cd $PIB_BASE
git add .
git commit -am "added flagger-grafana app"
git push

cd apps/flagger-grafana

# check deploy targets (should be [])
flt targets list

# if not [], clear the targets
# flt targets clear

# add all clusters as a target
flt targets add all

# deploy the changes
flt targets deploy

```

Once the [GitHub action](https://github.com/kubernetes101/pib-dev/actions) is completed and flux sync
is performed, navigate to grafana dashboard by appending `/grafana` to the host url in the browser tab.

The default Grafana login info is a user name of `admin` with a password of `change-me`.

![An image of an example Canary Dashboard on Grafana. There are a few graphs indicating traffic metrics.
On the left hand side of the image there are the metrics for the "Primary" Deployment, where previous
activity is visible. On the right hand side of the image, there are the graphs and metrics from the
canary deployment, which should no current activity.](../../images/envoyCanaryDashboard.png)
