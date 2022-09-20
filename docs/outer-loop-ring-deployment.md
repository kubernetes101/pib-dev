# PiB outer-loop with Ring Based Deployment

## Create 15 Clusters

> Note: we don't actually create the clusters, just the GitOps folders

    ```bash

    # start in the base of the repo
    cd $PIB_BASE

    flt create \
        --gitops-only \
        -g $MY_BRANCH-fleet \
        -c central-tx-atx-101 \
        -c central-tx-dal-101 \
        -c central-tx-hou-101 \
        -c central-tx-ftw-101 \
        -c central-tx-san-101 \
        -c east-ga-atl-101 \
        -c east-fl-mia-101 \
        -c east-al-bham-101 \
        -c east-ms-bil-101 \
        -c east-nc-clt-101 \
        -c west-wa-sea-101 \
        -c west-nv-lv-101 \
        -c west-ca-sd-101 \
        -c west-or-pdx-101 \
        -c west-mt-bose-101

    ```

## Update Git Repo

- `flt create` generates GitOps files for the cluster
- [CI-CD](https://github.com/kubernetes101/pib-dev/actions) generates the deployment manifests
  - Wait for CI-CD to complete (usually about 30 seconds)

```bash

# update the git repo after ci-cd completes
git pull

```

## Add Metadata to Clusters

```bash
```

