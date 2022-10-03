# Pilot-in-a-Box Overview

Kubernetes is hard. Getting started and set up for the first time can take weeks to get right. Managing deployments on a fleet of Kubernetes clusters on the edge brings even more challenges.

Pilot-in-a-Box (PiB) is a `game-changer` for the end-to-end Kubernetes app development cycle from a local cluster to deployments on the edge. It reduces the initial friction and empowers the developer to get started and deployed to a dev/test environment within *minutes*. The pre-configured Codespaces environment includes a `Kubernetes` cluster and custom CLI's (`kic` and `flt`) that help streamline the initial learning curve to Kubernetes development commands.

This repo walks through the rich end-to-end developer experience in a series of labs. The labs start by walking you through creating, building, testing, and deploying an application on a local cluster ([inner-loop](./README.md#inner-loop)) with a complete CNCF observability stack. Then, the labs move on to the next step of deploying the application to a test cluster in the Cloud ([outer-loop](./README.md#outer-loop)). There are also several [advanced labs](./README.md#advanced-labs) that cover centralized monitoring, canary deployments, and targeting different devices.

> Note: PiB is not intended as-is for production deployments. However, some concepts covered (GitOps and Observability) are production-ready.

## Prerequisites

- GitHub Codespaces access
- An Azure subscription with owner access
  - A [free Azure subscription](https://azure.microsoft.com/en-in/free/) will work for exploration
  - Some advanced scenarios also require AAD permissions

- For pre-release access
  - Reach out to the SolDevEx team
  - We can provide Codespaces access during pre-release

## GitHub Codespaces

> Codespaces allows you to develop in a secure, configurable, and dedicated development environment in the cloud that works how and where you want it to

- [GitHub Codespaces Overview](https://docs.github.com/en/codespaces)
  - GitHub Codespaces is available for organizations using GitHub Team or GitHub Enterprise Cloud. GitHub Codespaces is also available as a limited beta release for individual users on GitHub Pro plans.
  - For more information, see ["GitHub's products"](https://docs.github.com/en/get-started/learning-about-github/githubs-products)

We use GitHub Codespaces for our `inner-loop` and `outer-loop` Developer Experiences. While other DevX are available, currently, we only support GitHub Codespaces.

The easiest way to get GitHub Codespaces access is to setup a [GitHub Team](https://docs.github.com/en/codespaces)

GitHub Codespaces is also available in beta on a limited basis for GitHub Pro users. The waiting list is normally > 3 months.

> Best Practice: as you begin projects, ensure that you have Codespaces and Azure subscriptions with proper permissions

## inner-loop

- `inner-loop` refers to the tasks that developers do every day as part of their development process
  - Generally, `inner-loop` happens on the individual developer workstation
    - For PiB, the inner-loop and developer workstation is Codespaces
    - When a developer creates a Codespace, that is their "personal development workstation in the cloud"
- As part of PiB, we have automated the creation of the developer workstation using a repeatable, consistent, Infrastructure as Code approach
  - We have an advanced workshop planned for customizing the Codespaces experience for your project
- With the power of Codespaces, a developer can create a consistent workstation with a few clicks in less than a minute

## outer-loop

- `outer-loop` refers to the tasks that developers and DevOps do as they move from dev to test to pre-prod to production
  - Generally `outer-loop` happens on shared compute outside of the developer workstation
  - For PiB, outer-loop uses a combination of Codespaces and `dev/test clusters` in Azure
- As part of PiB, we have automated the creation of dev/test clusters using a repeatable, consistent, Infrastructure as Code approach

## Create a Codespace

> You can use the same Codespace for any of the labs

- From this repo
  - Click the `<> Code` button
    - Make sure the Codespaces tab is active
  - Click `Create Codespace on main`
- After about 1 minute, you will have a GitHub Codespace running with a complete Kubernetes Developer Experience!

## Create a working branch

- Because the main branch has a branch protection rule, you need to create a working branch
  - You can use the same branch for any of the labs or create a new branch per lab (add 1, 2, 3 ... to the branch name)

  ```bash

  # use lower case GitHub User Name as branch name
  export MY_BRANCH=$(echo $GITHUB_USER | tr '[:upper:]' '[:lower:]')
  echo $MY_BRANCH

  # create a branch
  git checkout -b $MY_BRANCH

  # push the branch and set the remote
  git push -u origin $MY_BRANCH

  ```

- Your prompt should look like this
  - /workspaces/pib-dev (mybranch) $

## inner-loop Labs

- [Lab 1](./labs/inner-loop.md): Create, build, deploy, and test a new dotnet application and observability stack on your local cluster
- [Lab 2](./labs/inner-loop-flux.md): Configure flux to automate the deployment process from Lab 1

## outer-loop Labs

- [Lab 1](./labs/outer-loop.md): Create a dev/test cluster and manage application deployments on the cluster
- [Lab 2](./labs/outer-loop-multi-cluster.md): Manage application deployments on a fleet of multiple clusters
- [Lab 3](./labs/outer-loop-ring-deployment.md): Configure ring based deployments

- [Lab 4](./labs/azure-codespaces-setup.md): Set up Azure subscription and Codespaces for advanced configuration
  - This is a prerequisite for the Advanced Labs

## Advanced Labs

- [Arc enabled GitOps Lab](./labs/outer-loop-arc-gitops.md): Deploy to dev cluster running on an Azure VM with Arc enabled GitOps
- [Canary Deployment Lab](./labs/advanced-labs/canary/README.md): Use Flagger to experiment with canary deployments
- [Vision on Edge (VoE) Lab](./labs/advanced-labs/voe/README.md): Deploy a more complex app (VoE) to a fleet
- [Centralized Observability Lab](./labs/advanced-labs/monitoring/README.md): Deploy a centralized observability system with Fluent Bit, Prometheus, and Grafana to monitor fleet application deployments
- [outer-loop with AKS-IoT](./labs/advanced-labs/aks-iot/README.md): Deploy to an AKS-IoT cluster running on an Azure VM with Arc enabled GitOps
- [outer-loop with AKS Lab](./labs/outer-loop-aks-azure.md): Deploy to an AKS cluster with Arc enabled GitOps

## Cleanup

- Once you are finished with all of the labs and experimenting, please delete your branch

  ```bash

  # change to the root of the repo
  cd $PIB_BASE

  git pull
  git add .
  git commit -am "deleting branch"
  git push

  # checkout main branch, delete remote, delete local
  git checkout main
  git push origin $MY_BRANCH --delete
  git branch -D $MY_BRANCH

  ```

## Support

This project uses GitHub Issues to track bugs and feature requests. Please search the existing issues before filing new issues to avoid duplicates.  For new issues, file your bug or feature request as a new issue.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit <https://cla.opensource.microsoft.com>.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft
trademarks or logos is subject to and must follow [Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
