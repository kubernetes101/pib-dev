## flt create

Create a new cluster

```
flt create [flags]
```

### Options

```
  -a, --arc                Connect kubernetes cluster to Azure via Azure ARC
  -c, --clusters strings   Kubernetes cluster name(s) (required)
      --cores int          VM core count (default 4)
      --dapr               Install Dapr on the cluster
      --dns-group string   DNS Resource Group (default "tld")
      --dry-run            Show values that would be used
      --gitops-only        Only generate GitOps targets in ./clusters
  -g, --group string       Azure resource group name
  -h, --help               help for create
  -k, --keyvault string    Azure Key Vault Name (default "pib-kv")
  -l, --location string    Azure location (default "westus3")
  -n, --no-setup           Create VM without executing setup steps
      --osm                Enable Open Service Mesh (requires --arc)
      --sku string         Azure VM SKU (default "Standard_D4as_v5")
  -s, --ssl string         DNS domain name (default "cseretail.com")
```

### SEE ALSO

* [flt](flt.md)	 - Kubernetes Fleet CLI

###### Auto generated by spf13/cobra on 23-Sep-2022
