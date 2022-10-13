# PiB - VM Setup and Operations

This directory contains the setup and operations scripts used by `flt` to create and operate `dev/test clusters`

- scripts
  - These scripts get executed via SSH on the cluster
    - For example: `flt check flux` executes the `vm/scripts/check-flux` bash script on each cluster in the fleet
- setup
  - The `vm/setup/setup.sh` script is executed as part of `cloudinit`
    - The scripts are broken into `stages` and can be customized
    - The scripts run in the `pib` user context - `sudo` is available
