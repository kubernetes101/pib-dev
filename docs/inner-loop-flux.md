# PiB inner-loop with GitOps (Flux)

## Create a New Cluster

> The k3d cluster will run `in` your Codespace - no need for an external cluster

- Use `kic` to create and verify a new k3d cluster

```bash

# delete and create a new cluster
kic cluster create

# wait for pods to get to Running
kic pods --watch

```

## Create a new .NET WebAPI

> You can skip this step if you already created MyApp

- PiB includes templates for new applications that encapsulate K8s best practices
  - For the Workshop, use `MyApp` for the application name
  - You can use any app name that conforms to a dotnet namespace
    - PascalCase
    - alpha only
    - <= 20 chars
- Once created, you can browse the code in the Explorer window

```bash

# create a new app from the dotnet web api template
cd apps
kic new dotnet-webapi MyApp

# this is important as the CLI is "context aware"
cd myapp

```

## Build MyApp

> Make sure you are in the apps/myapp directory

- Now that we've created a new application, the next logical step is to `build` the app
- PiB encapsulates many best practices, so, as an App Dev, you don't have to figure out how to build a secure, multi-stage Dockerfile

```bash

# build the app
kic build all

# view docker images
docker images

```

## Deploy MyApp and Observability with GitOps (Flux v2)

> Notice you don't have to create or edit K8s YAML files!

- This deploys
  - Flux
  - MyApp
  - Fluent Bit
  - Grafana
  - Prometheus
  - WebValidate (more on this later)

```bash

# deploy flux to the cluster
kic cluster flux-install

# wait for pods to start
kic pods --watch

# check flux
kic check flux

```

## Force Flux to Sync

- After making changes, you can force Flux to sync (reconcile)

```bash

kic sync

```

## Flux Setup Files

- The Flux setup yaml is located in `apps/myapp/kic-deploy/flux`
  - A `Flux source` is a git repo / branch combination
  - A `Flux kustomization` is a directory within the source
    - We have 3 kustomizations
      - kustomization-flux watches the flux directory
      - kustomization-app watches the app directory
      - kustomization-monitoring watches the monitoring directory
    - You want to have multiple kustomizations (or helm)
      - When a kustomization fails, the entire process is aborted
        - This lets "your app" break "my app"
      - We generally create a kustomization per namespace for production
        - Our GitOps Automation (outer-loop) automatically creates a kustomization per namespace
- View the flux setup script by running `kic cluster flux-install --show`
  - Do NOT run this fence!

    ```bash

    # create the namespace
    kubectl apply -f namespace.yaml

    # create the flux secrets from the GitHub PAT
    flux create secret git flux-system -n flux-system --url "$PIB_FULL_REPO" -u gitops -p "$PIB_PAT"

    # deploy Flux
    kubectl apply -f controllers.yaml

    # create a "source" that points to this repo / branch
    kubectl apply -f source.yaml

    # create a "kustomization"
    # this kustomization will synchronize all files in the flux directory
    # including kustomization-app.yaml
    #           kustomization-monitoring.yaml
    kubectl apply -f kustomization-flux.yaml

    ```

## Testing

- Use the [inner-loop](./inner-loop.md#check-the-k8s-pods) instructions to further test the deployment
