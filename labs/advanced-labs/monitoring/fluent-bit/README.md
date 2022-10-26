# Fluent Bit (with Loki) Setup

## Grafana Cloud Configuration

### Create Fluent Bit Secret

The Fluent Bit deployment expects to retrieve the value for the Grafana Cloud API Key from a kubernetes
(k8s) secret. In order to achieve this, we store the value as a secret in Key Vault. Each member of
the fleet retrieves the value from Key Vault during setup and creates the needed secret on the cluster.

To start you need to go to [grafana.com](https://grafana.com) and log in. Instructions on how to sign
up are found [here](/labs/advanced-labs/monitoring/README.md#fleet-configuration-prerequisites).
Once you login, go to the settings in `My Account`. You will get redirected to a URL with a similar
format to `https://grafana.com/orgs/yourUserName`. In the left nav bar, click on `API Keys` (under
Security), and click `+ Add API Key`.

From here you'll need to name your API key (i.e. something like `yourName-publisher`). Choose the
`MetricsPublisher` as the role and then click `Create API Key`.

Get the generated Personal Access Token (PAT), saving the value wherever you can retrieve it. Warning!
You will not be able to get back to this value later from Grafana! Below we'll save it as an environment
variable.

```bash
GC_PAT="<Paste your API Key value here>"
```

You'll then save to Key Vault using the AzCLI and the newly created PAT environment variable.

```bash
az keyvault secret set --vault-name $PIB_KEYVAULT --name fluent-bit-secret --value ${GC_PAT}
```

### Update Fluent Bit Config

Before running Fluent Bit on your monitoring cluster, you need to update the values in `/apps/fluent-bit/app.yaml`
to match your fleet and Grafana Cloud instance. The following values need to be set: `jobSuffix`, `lokiUrl`,
`lokiUser`.

#### `jobSuffix`

This value is the name of your fleet and will be used to uniquely identify the logs from this instance
in Loki queries. For example, if your fleet name is `atx-fleet`, jobSuffix should be `atx`.

#### `lokiUser` and `lokiHost`

These values are located in the Grafana Cloud Portal, at a URL similar to `https://grafana.com/orgs/yourUserName`.
Click `Details` in the `Loki` section, then under the Grafana Data Source Settings, set the values.
Set `lokiHost` to the `URL` value (remove the leading "https://" protocol), and `lokiUser` to the `User`
value.

## Fluent Bit Configuration

You can see the configuration yaml file: [fluent-bit.yaml](./.gitops/dev/fluent-bit.yaml). For more
information on Fluent Bit, see their [documentation](https://docs.fluentbit.io/manual/concepts/data-pipeline).

### Inputs

By default, the inputs are logs from containers named `webv*`. To use logs from other apps, you will
need to create a new input block and update the Path parameter to match the new app container name.

### Parsers

This configuration uses built-in parsers (cri, docker) to parse the container logs for forwarding.

### Filters

This configuration uses a few filters to enrich and control the logs. The kubernetes filter is used
to add kubernetes metadata to the logs. The nest filters apply the lift operation to the logs to lift
nested labels up to simplify querying. The `type_converter` and `grep` filters are used to ensure only
error logs are forwarded.

This is not required, but a safety net to not unintentionally forward all logs if a verbose flag is
accidentally set on a deployment.

### Outputs

With the default configuration provided here, the Fluent Bit instance will forward the processed logs
from `webv` to Grafana Loki. To forward logs from other apps, you will need to create a new output
block and update the Match, Labels, label-keys, and remove-keys to reflect the naming and log structure
of the new app.

Leverage the Fluent Bit output plugin [documentation](https://docs.fluentbit.io/manual/pipeline/outputs)
to explore different output options.
