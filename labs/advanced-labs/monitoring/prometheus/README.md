# Prometheus Setup

## Grafana Cloud Configuration

- The Prometheus deployment expects to retrieve the value for the Grafana Cloud API Key from a kubernetes secret.
- To acheive this, we store the value as a secret in Key Vault.
- Each member of the fleet retrieves the value from Key Vault during setup and creates the needed secret on the cluster.

### Create Prometheus Secret

- Go to <https://grafana.com> and log in
- Click on `My Account`
  - You will get redirected to this URL <https://grafana.com/orgs/yourUserName>
- Click on `Details` in the `Prometheus` section
- Click on `Generate now` under the `Password / API Key` section to generate a new password
  - Name your API Key (i.e. yourName-publisher)
  - Select `MetricsPublisher` as the role
  - Click on `Create API Key`
  - Click on `Copy to Clipboard` and save the value wherever you save your PATs
    - WARNING - you will not be able to get back to this value
- In the Section `Prometheus remote_write Configuration`
  - Copy the value of the password in the config

```bash

GC_PROM_PASSWORD="<Paste value here>"

```

Save as Key Vault secret

```bash

az keyvault secret set --vault-name $PIB_KEYVAULT --name prometheus-secret --value $GC_PROM_PASSWORD

```

### Update Prometheus Config

- Before deploying Prometheus to your monitoring cluster, you need to update the values in `/apps/prometheus/app.yaml` to match your Grafana Cloud instance.
- The following values need to be set:
  - prometheusURL
  - prometheusUser

#### prometheusURL and prometheusUser

These values are located in the Grafana Cloud Portal.

- Go to the `Grafana Cloud Portal`: <https://grafana.com/orgs/yourUserName>
- Click `Details` in the `Prometheus` section
- Under Grafana Data Source Settings:
  - Set prometheusURL to the `Remote Write Endpoint` value
  - Set prometheusUser to the `Username / Instance ID` value

## Prometheus Configuration

- The "origin_prometheus" value in the [Prometheus configuration](./.gitops/dev/prometheus.yaml) is important as it serves as a way to uniquely identify the source of the metrics when querying in Grafana Cloud.
- By default, this will be set to the name of the store that Prometheus is deployed to.

```yaml

    global:
      scrape_interval: 5s
      evaluation_interval: 5s
      external_labels:
        origin_prometheus: {{gitops.cluster.store}}

```

- The scrape configs specify what targets to scrape metrics from and which metrics to keep or drop.
- There is a scrape job named "webv-heartbeat" that scrapes metrics from the webv-heartbeat app running on the cluster.
- The metrics WebVDuration_count, WebVSummary, and WebVSummary_count are the only metrics configured to be kept by default since they are the only ones being used by our dashboard queries.

```yaml

    scrape_configs:
      - job_name: 'webv-heartbeat'
        static_configs:
          - targets: [ 'webv-heartbeat.webv.svc.cluster.local:8080' ]
        metric_relabel_configs:
        - source_labels: [ __name__ ]
          regex: "WebVDuration_count|WebVSummary|WebVSummary_count"
          action: keep

```

- For more information on Prometheus configuration, see their [documentation](https://prometheus.io/docs/prometheus/latest/configuration/configuration/).
