# Pilot in a Box

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

- inner-loop [Lab 1](docs/inner-loop.md)
- inner-loop with GitOps [Lab 2](docs/inner-loop-flux.md)

## outer-loop Labs

- outer-loop [Lab 1](docs/outer-loop.md)
- Multi-cluster outer-loop [Lab 2](docs/outer-loop-multi-cluster.md)
- Ring Based Deployment [Lab 3](docs/outer-loop-ring-deployment.md)

> Work in Progress

- Setting up Azure subscription and Codespaces [Lab 4](docs/azure-codespaces-setup.md)
  - This is a prerequisite for the Advanced Labs

## Advanced Labs

> Work in Progress

- Arc enabled Gitops [Lab](docs/outer-loop-arc-gitops.md)
- Canary Deployment [Lab](advanced-scenarios/canary/README.md)
- Vision on Edge (VoE) [Lab](advanced-scenarios/voe/README.md)
- Centralized Observability [Lab](advanced-scenarios/monitoring/README.md)
- outer-loop with Physical Devices [Lab](docs/outer-loop-physical-devices.md)
- outer-loop with AKS [Lab](docs/outer-loop-aks-azure.md)

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

This project welcomes contributions and suggestions and has adopted the [Contributor Covenant Code of Conduct](https://www.contributor-covenant.org/version/2/1/code_of_conduct.html).

For more information see [Contributing.md](./.github/CONTRIBUTING.md)

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Any use of third-party trademarks or logos are subject to those third-party's policies.
