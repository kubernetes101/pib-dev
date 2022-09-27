# PiB outer-loop with Ring Based Deployment

- PiB includes `GitOps Automation` that uses `cluster metadata` for targeted deployments
- In this lab we will
  - Create the GitOps structure for 15 clusters
  - Add `ring` metadata to each cluster
  - Add targets based on cluster metadata

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

## Cluster Metadata Files

```bash

ls -alF clusters/*.yaml

cat clusters/central-tx-atx-101.yaml

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

echo "ring: 0" >> clusters/central-tx-atx-101.yaml
echo "ring: 1" >> clusters/central-tx-dal-101.yaml
echo "ring: 2" >> clusters/central-tx-ftw-101.yaml
echo "ring: 3" >> clusters/central-tx-hou-101.yaml
echo "ring: 4" >> clusters/central-tx-san-101.yaml
echo "ring: 0" >> clusters/east-al-bham-101.yaml
echo "ring: 1" >> clusters/east-fl-mia-101.yaml
echo "ring: 2" >> clusters/east-ga-atl-101.yaml
echo "ring: 3" >> clusters/east-ms-bil-101.yaml
echo "ring: 4" >> clusters/east-nc-clt-101.yaml
echo "ring: 0" >> clusters/west-ca-sd-101.yaml
echo "ring: 1" >> clusters/west-mt-bose-101.yaml
echo "ring: 2" >> clusters/west-nv-lv-101.yaml
echo "ring: 3" >> clusters/west-or-pdx-101.yaml
echo "ring: 4" >> clusters/west-wa-sea-101.yaml

git add clusters

```

## Deploy IMDb to ring:0

```bash

cd $PIB_BASE/apps/imdb
flt targets clear
flt targets add ring:0
flt targets deploy

# wait for ci-cd to finish
git pull

```

## Add ring:1

```bash

cd $PIB_BASE/apps/imdb
flt targets add ring:1
flt targets deploy

# wait for ci-cd to finish
git pull

```

## Add Central Region

```bash

cd $PIB_BASE/apps/imdb
flt targets add region:central
flt targets deploy

# wait for ci-cd to finish
git pull

```

## Clean Up

- Once you are finished with the workshop, you can delete your GitOps resources

```bash

# start in the base of the repo
cd $PIB_BASE
git pull

rm -rf clusters
mkdir -p clusters
touch clusters/.gitkeep
git add .

cd apps/imdb
flt targets clear
flt targets deploy
cd ../..

```
