# Centralized Monitoring

## Introduction

- To monitor a multi-cluster fleet, we deploy a central monitoring cluster with Fluent Bit and Prometheus configured to send logs and metrics to Grafana Cloud.
- The monitoring cluster runs WebValidate (WebV) to send requests to apps running on the other clusters in the fleet. - The current design has one deployment of WebV for each app.
  - The webv-heartbeat deployment sends requests to all of the heartbeat apps running on the fleet clusters.
- Fluent Bit is configured to forward WebV logs to Grafana Loki
- Prometheus is configured to scrape WebV metrics.
- These logs and metrics are used to power a Grafana Cloud dashboard and provide insight into cluster and app availability and latency.

## Lab Prerequisites

- Complete the 4 [outer-loop labs](/README.md#outer-loop-labs) before this one.

## Fleet Configuration Prerequisites

- Grafana Cloud Account
  - You can set up a free trial Grafana Cloud Account [here](https://grafana.com/).
- Azure subscription
- Managed Identity (MI) for the fleet
- Key Vault
  - Grant the MI access to the Key Vault

## Key Vault Secrets

- A PAT is required to forward logs and metrics to Grafana Cloud.
- The PAT is stored as a k8s secret on the fleet clusters.
- Before creating the secrets, a Key Vault and MI (with access to the Key Vault) must be configured. See [setup docs](/docs/azure-codespaces-setup.md) for instructions.

### Fluent Bit Secret

Follow instructions [here](./fluent-bit/README.md#create-fluent-bit-secret) to create the required Fluent Bit secret in the Key Vault.

### Prometheus Secret

Follow instructions [here](./prometheus/README.md#create-prometheus-secret) to create the required Prometheus secret in the Key Vault.

### Execution

- The Key Vault secret values are retrieved (via MI) during fleet creation and stored as kubernetes secrets on each cluster in the fleet (in [azure.sh](../vm/setup/azure.sh#L36) and [pre-flux.sh](../vm/setup/pre-flux.sh#L29)).
- The logging (fluent-bit) and metrics (prometheus) namespaces are bootstrapped on each of the clusters, prior to secret creation.

## Deploy a Central Monitoring Cluster

> This assumes you have an existing [multi-cluster fleet](/docs/outer-loop-multi-cluster.md).

```bash

# set to the name of your fleet
# these commands assume the resource group for your fleet is named $FLT_NAME-fleet
export FLT_NAME=yourfleetname

# check required env vars
# PIB_MI and PIB_KEYVAULT need to be set to the appropriate values
flt env

flt create -g $FLT_NAME-fleet -c monitoring-$FLT_NAME

```

## WebV

### Create apps/webv Directory

- Add webv to the apps/ directory
- By default, this provides you with two deployments of webv: webv-heartbeat and webv-imdb.

```bash

# make sure you are in the monitoring directory
cd $PIB_BASE/advanced-scenarios/monitoring

cp -aR ./webv ../../apps

# commit changes
git add ../../
git commit -m "Adding webv to apps dir"
git push

```

### Configure WebV

- Before deploying, you need to update the arguments for the webv-heartbeat and webv-imdb deployments to target the clusters in your fleet.
- Replace the server arguments (placeholders are <https://yourclustername.yourdomain.com>) with the fqdn for the clusters in your fleet. You can find these values in the cluster yaml metadata files in the clusters/ directory.
  - If you are not using dns, use the cluster IP's
  - <http://yourclusterIP>

```yaml

          args:
          - --sleep
          - "5000"
          - --prometheus
          - --run-loop
          - --server
          - https://yourclustername.yourdomain.com
          - https://yourclustername2.yourdomain.com
          - --files
          - heartbeat-load.json
          - --zone
          - {{gitops.cluster.zone}}
          - --region
          - {{gitops.cluster.region}}
          - --log-format
          - Json

```

### Deploy WebV to the Central Monitoring Cluster

```bash

# make sure you are in the apps/webv directory
cd $PIB_BASE/apps/webv

# should be empty
flt targets list

# clear if needed
flt targets clear

# add monitoring cluster
flt targets add region:monitoring

flt targets deploy

```

### Verify WebV was Deployed

```bash

# should see webv added
git pull

# force flux to reconcile
flt sync

# should be found on the monitoring cluster
flt check app webv

# should see webv pods running
flt exec kic pods -f monitoring

```

## Fluent Bit

### Create apps/fluent-bit Directory

- Add fluent-bit to the apps/ directory

```bash

cd $PIB_BASE/advanced-scenarios/monitoring

cp -aR ./fluent-bit ../../apps

# commit changes
git add ../../
git commit -m "Adding fluent-bit to apps dir"
git push

```

### Configure Fluent Bit

Follow the instructions [here](./fluent-bit/README.md#update-fluent-bit-config) to configure the Fluent Bit deployment.

### Deploy Fluent Bit to the Central Monitoring Cluster

```bash

# make sure you are in the fluent-bit directory
cd $PIB_BASE/apps/fluent-bit

# should be empty
flt targets list

# clear if needed
flt targets clear

# add monitoring cluster
flt targets add region:monitoring

flt targets deploy

```

### Verify Fluent Bit was Deployed

```bash

# should see fluent-bit added
git pull

# force flux to reconcile
flt sync

# should be found on the monitoring cluster
flt check app fluent-bit

# should see fluent-bit pod running
flt exec kic pods -f monitoring

```

## Prometheus

### Create apps/prometheus directory

- Add prometheus to the apps/ directory

```bash

cd $PIB_BASE/advanced-scenarios/monitoring

cp -aR ./prometheus ../../apps

# commit changes
git add ../../
git commit -m "Adding prometheus to apps dir"
git push

```

### Configure Prometheus

Follow the instructions [here](./prometheus/README.md#update-prometheus-config) to configure the Prometheus deployment.

### Update targets and deploy Prometheus to the central monitoring cluster

```bash

# make sure you are in the prometheus directory
cd $PIB_BASE/apps/prometheus

# should be empty
flt targets list

# clear if needed
flt targets clear

# add monitoring cluster
flt targets add region:monitoring

flt targets deploy

```

### Verify Prometheus was Deployed

```bash

# should see prometheus added
git pull

# force flux to reconcile
flt sync

# should be found on the monitoring cluster
flt check app prometheus

# should see prometheus pod running
flt exec kic pods -f monitoring

```

## Create Grafana Cloud Dashboard

```bash

# set to dns or no-dns depending on your fleet configuration
export DASHBOARD_TYPE=dns

# make sure $PIB_SSL is set to your domain name
# can skip if dns/ssl is not configured
echo $PIB_SSL

# set environment variable to your grafana cloud account name
export GRAFANA_NAME=yourgrafanacloudaccountname

# cd to this directory
cd $PIB_BASE/advanced-scenarios/monitoring

# generate json based on dashboard-template-dns
cp dashboard-template-$DASHBOARD_TYPE.json dashboard.json
sed -i "s/%%FLEET_NAME%%/${FLT_NAME}/g" dashboard.json
sed -i "s/%%DOMAIN_NAME%%/${PIB_SSL}/g" dashboard.json
sed -i "s/%%GRAFANA_NAME%%/${GRAFANA_NAME}/g" dashboard.json

```

- Copy the content in dashboard.json and import as a new dashboard in Grafana Cloud.

## Create Grafana Alert

- Go to Grafana Cloud > Alerting > Alert Rules
- Create a new alert (+ New Alert Rule)
  - Rule type: Grafana managed alert
- Query A:
  - Select grafanacloud.yourgrafananame.prom as the source from the drop down list
  - Replace [your $FLT_NAME] with your fleet name and copy the query below to the query field

  ```sql

  sum(rate(WebVDuration_count{status!="OK",server!="",origin_prometheus="monitoring-[your $FLT_NAME]"}[10s])) by (server,job) / sum(rate(WebVDuration_count{server!="",origin_prometheus="monitoring-[your $FLT_NAME]"}[10s])) by (server,job) * 100

  ```

- Query B:
  - Set Operation to Reduce
  - Set Function to Last
  - Set Input to A
  - Leave Mode as Strict
- Add another Expression (+ Add expression)
  - Name the Expression "More than 5% errors"
  - Set Operation to Math
  - Type in the Expression: $B > 5
- Set alert condition to "More than 5% errors"
- Set alert evaluation behavior
  - Set evaluate every to 30s
  - Set for to 1m
- Add details for your alert
  - Replace [your $FLT_NAME] with your fleet name
    - Rule name: [your $FLT_NAME] App Issue
    - Folder: Pick any folder
    - Group: [your $FLT_NAME] - App Issue
- Click 'Save and exit'

## "Break" an Application Deployment

- To watch the dashboard and alerts "in action", we will temporarily take down an instance of imdb

### Reduce imdb Targets

```bash

cd $PIB_BASE/apps/imdb

# show current targets
flt targets list

# clear targets
flt targets clear

# target only one region/cluster
# update accordingly depending on the clusters in your fleet
flt target add region:central

flt targets deploy

```

### Update Cluster

```bash

# should see imdb removed from some clusters
git pull

# force flux to reconcile
flt sync

```

- Navigate to your dashboard in Grafana to see some clusters "turn red"
  - You may need to hit refresh a few times
  - The alert will take ~1 minute to show up
