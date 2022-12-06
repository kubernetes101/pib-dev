# PiB CLI Customization

PiB ships two CLIs (kic and flt) to make learning and working with Kubernetes easier. Both CLIs can be customized on an organization, team, or app level. This workshop walks through the different customization options with examples.

## Create Working Branch

```bash

git checkout -b $MY_BRANCH-cli
git push -u origin $MY_BRANCH-cli

```

## CLI Customization

- There are 3 options that allow customization of the CLI
  - Overriding built-in commands using bash scripts
  - Adding / Removing commands using `boa` files
  - Modifying the source code
- The CLIs also support custom tab completion
- This lab does not cover modifying the source code

## Labs

- Start in the `/workspaces/pib-dev/labs/advanced-labs/cli` directory
  - This is the base directory for all of the labs

  ```bash

  cd /workspaces/pib-dev/labs/advanced-labs/cli
  kic pods --show

  ```

- Output

  ```text

  Create this file to override command
  /workspaces/pib-dev/labs/advanced-labs/cli/.kic/commands/pods

  Default Script

  #!/bin/bash

  hdrsort()
  {
      read -r
      printf "%s\\n" "$REPLY"
      sort
  }

  if [ "$1" != "--watch" ]; then
          kubectl get pods -A | hdrsort
  else
          kubectl get pods -A --watch
  fi

  ```

- We have already overriden the command

  ```bash

  kic pods

  code /workspaces/pib-dev/labs/advanced-labs/cli/.kic/commands/pods

  # change the file and run kic pods again

  ```

- The `.kic` (or `.flt`) directory stores all of the CLI customizations
  - Default search order
    - Current tree
    - $HOME/bin/.kic

- We created the `.kic` folder in this tree

## Context Aware CLI

- The CLI is also context aware

  ```bash

  # notice the output
  kic

  # different output
  cd myapp
  kic

  ```

- There is a `.kic` folder in the `myapp` folder that changes the behavior
  - There are 3 new commands in `myapp/.kic/commands/check`
  - Run `kic check` and you'll see 5 commands
  - `cd ..` and you'll see there is no `kic check` command (because there is no "app")
    - Running `kic check` in this directory will cause an error

## Adding Commands

- From the `myapp` directory

  ```bash

  code .kic/check.yaml

  ```

- This creates the `kic check` command
  - It adds 3 subcommands (myapp, webv, and flux)
    - The commands run the script at `.kic/commands/{path}`

- Edit `.kic/commands/check/app`
  - Replace the http command with `echo "kic check app"`
  - Run `kic check myapp`

- Edit `.kic/check.yaml`
  - Change `name: myapp` to `name: app`
  - Run `kic check`
  - Run `kic check app`
  - Run `kic check myapp`
    - Notice it fails

## Add New Commands

- Edit `.kic/check.yaml`
- Insert the following directly beneath `commands:`

```yaml

commands:

// insert this below the existing commands:

- name: foo
  short: Foo Command
  bashCommands:
    - name: bar
      short: Foo Bar Command
      command: echo "kic foo bar"

```

- Run `kic`
  - Notice the new `foo` command
- Run `kic foo`
  - Notice the new `bar` command
- Run `kic foo bar`

> Try out tab completion - it works on your new foo/bar command

## Hiding Commands

- Start in `/workspaces/pib-dev/labs/advanced-labs/cli/myapp`
- Run `kic`
- Notice the `build command`
- Edit `.kic/check.yaml`

```yaml

commands:

// insert below `commands:`

- name: build
  hidden: true

```

- Run `kic`
- Run `kic build`
  - Notice it fails

## Tab Completion

- Start in the `/workspaces/pib-dev/labs/advanced-labs/cli` directory
- Enter the following command and then press space and tab
  - `flt curl` space tab
- Edit the completion file
  - `code .flt/flt-curl-completion`
- The file is a tab delimited file that you can update
  - Note that Codespaces sometimes changes tab to spaces - it must be a tab
    - Copy paste a line or edit an existing line
- There is a `flt-check-app-completion` and `flt-targets-add-completion` in the same directory
- General naming is `cli-command-[subcommand-][subcommand-]completion`

## Create Your Own CLI

- Start in the `/workspaces/pib-dev/labs/advanced-labs/cli/mycli` directory
- Run `kic`
- Explore the `.kic` directory
