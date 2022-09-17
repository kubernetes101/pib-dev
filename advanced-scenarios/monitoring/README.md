# Centralized Monitoring

To monitor a fleet, we deploy a central monitoring cluster with fluent bit and prometheus configured to send logs and metrics to Grafana Cloud. The monitoring cluster runs WebValidate (WebV) to send requests to apps running on the other clusters in the fleet. The current design has one deployment of WebV for each app. For instance, the webv-heartbeat deployment sends requests to all of the heartbeat apps running on the fleet clusters.  Fluent Bit is configured to forward WebV logs to Grafana Loki and prometheus is configured to scrape WebV metrics.  These logs and metrics are used to power a Grafana Cloud dashboard and provide insight into cluster and app availability and latency.

## Prerequisites

* Grafana Cloud Account
* Azure subscription
* Managed Identity (MI) for the fleet
* Key Vault
  * Grant the MI access to the Key Vault

> TODO: document keyvault and MI creation

## Key Vault Secrets

### Fluent Bit Secret

Follow instructions [here](./fluent-bit/README.md#create-fluent-bit-secret) to create the required Fluent Bit secret in the Key Vault.

### Prometheus Secret

Follow instructions [here](./prometheus/README.md#create-prometheus-secret) to create the required Prometheus secret in the Key Vault.

### Execution

The Key Vault secret values are retrieved (via MI) during fleet creation and stored as kubernetes secrets on each cluster in the fleet (in [azure.sh](../vm/setup/azure.sh#L36) and [pre-flux.sh](../vm/setup/pre-flux.sh#L29)). Additionally, the logging (fluent-bit) and metrics (prometheus) namespaces are bootstrapped on each of the clusters, prior to secret creation.

## Deploy a central monitoring cluster to your existing fleet

> This assumes you have an existing multi-cluster fleet

```bash

# set to the name of your fleet
# these commands assume the resource group for your fleet is named $FLT_NAME-fleet
export FLT_NAME=yourfleetname

# check required env vars
flt env

# PIB_MI and PIB_KEYVAULT need to be set to the appropriate values
# TODO: Add instructions on setting these up

flt create -g $FLT_NAME-fleet -c monitoring-$FLT_NAME

```

## WebV

### Add WebV to apps/ directory

Copy the [WebV directory](./webv/) to the apps/ directory. By default, this provides you with two deployments of webv: webv-heartbeat and webv-imdb.

If you do not plan to deploy the imdb app to any stores in your fleet, it is recommended to not include the imdb.yaml. To remove, delete the `- imdb.yaml` line from the kustomization.yaml file.

```bash

# make sure you are in the monitoring directory
cd advanced-scenarios/monitoring

cp -aR ./webv ../../apps

# commit changes
git add ../../
git commit -m "Adding webv to apps dir"
git push

```

### Configure WebV

Before deploying, you need to update the arguments for the webv-heartbeat and webv-imdb deployments to target the clusters in your fleet.

Replace the server arguments (placeholders are https://yourclustername.yourdomain.com) with the fqdn or IP (if no dns) for the clusters in your fleet. You can find these values in the cluster yaml metadata files in the clusters/ directory.

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

### Update targets and deploy WebV to the central monitoring cluster

```bash

# make sure you are in the apps/webv directory
cd ../../apps/webv

# should be empty
flt targets list

# clear if needed
flt targets clear

# add monitoring cluster
flt targets add region:monitoring

flt targets deploy

# todo: validate deployment

```

## Fluent Bit

### Add Fluent Bit to apps/ directory

Copy the [fluent-bit directory](./fluent-bit/) to the apps/ directory in your fleet branch.

```bash

cd ../../advanced-scenarios/monitoring

cp -aR ./fluent-bit ../../apps

# commit changes
git add ../../
git commit -m "Adding fluent-bit to apps dir"
git push

```

### Configure Fluent Bit

Follow the instructions [here](./fluent-bit/README.md#update-fluent-bit-config) to configure the Fluent Bit deployment.

### Update targets and deploy Fluent Bit to the central monitoring cluster

```bash

# make sure you are in the fluent-bit directory
cd ../apps/fluent-bit

# should be empty
flt targets list

# clear if needed
flt targets clear

# add monitoring cluster
flt targets add region:monitoring

flt targets deploy

# todo: validate deployment

```

## Prometheus

### Add Prometheus to apps/ directory

Copy the [prometheus directory](./prometheus/) to the apps/ directory in your fleet branch.

```bash

cd ../../advanced-scenarios/monitoring

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
cd ../../apps/prometheus

# should be empty
flt targets list

# clear if needed
flt targets clear

# add monitoring cluster
flt targets add region:monitoring

flt targets deploy

# todo: validate deployment

```

## Create Grafana Cloud Dashboard

> TODO: Need to test
> TODO: Currently assumes dns is set up, need an option without dns

```bash

# make sure $PIB_SSL is set to your domain name
echo $PIB_SSL

# set environment variable to your grafana cloud account name
export GRAFANA_NAME=yourgrafanacloudaccountname

# cd to this directory
cd ../../advanced-scenarios/monitoring

# generate json based on dashboard-template
cp dashboard-template.json dashboard.json
sed -i "s/%%FLEET_NAME%%/${FLT_NAME}/g" dashboard.json
sed -i "s/%%DOMAIN_NAME%%/${PIB_SSL}/g" dashboard.json
sed -i "s/%%GRAFANA_NAME%%/${GRAFANA_NAME}/g" dashboard.json

```

Copy the content in dashboard.json and import as a new dashboard in Grafana Cloud.

## Create Alert for New Fleet

> TODO: Update and investigate if there is a better way to streamline

* Go to Grafana Cloud > Alerting > Alert Rules.
* Create a new alert (+ New Alert Rule).
  * Name the rule $FLT_NAME App Issue
  * Rule type: Grafana managed alert
  * Folder: Platform
  * Group: $FLT_NAME - App Issue

* Under "Create a query to be alerted on":
  * For Query A:
    * Select grafanacloud.retailedge.prom as the source from the drop down list.
    * Replace {your $FLT_NAME} with your fleet name and copy the query below to the query field.
  * For Query B:
    * Set Operation to Reduce
    * Set Function to Last
    * Set Input to A
    * Leave Mode as Strict
  * Add another Expression (+ Expression)
    * Name the Expression "More than 5% errors"
    * Set Operation to Math
    * Type in the Expression: $B > 5
* Under "Define alert conditions"
  * Set Condition to "More than 5% errors"
  * Set evaluate every to 30s
  * Set for to 1m

Alert Query:

```sql

sum(rate(WebVDuration_count{status!="OK",server!="",origin_prometheus="corp-monitoring-{your $FLT_NAME}"}[10s])) by (server,job) / sum(rate(WebVDuration_count{server!="",origin_prometheus="corp-monitoring-{your $FLT_NAME}"}[10s])) by (server,job) * 100

```
