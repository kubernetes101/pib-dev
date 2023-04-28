#!/bin/bash
# temporarily set $HOME
export PIB_ME=pib
export HOME="/home/$PIB_ME"
{
####### do not change these values #######
  echo 'export PIB_BASE="/workspaces/pib-dev"'
  echo 'export PIB_REPO="kubernetes101/pib-dev"'
  echo 'export PIB_FULL_REPO="https://github.com/kubernetes101/pib-dev"'
  echo 'export PIB_BRANCH="kev-ms"'
  echo 'export PIB_GHCR="ghcr.io/yourOrg"'
  # add command line params last so they don't get overwritten
  echo "export PIB_ME=$PIB_ME"
  echo "export PIB_IS_INNER_LOOP=false"
  echo 'export PIB_FQDN=outerloop-kms1'
  echo 'export PIB_NO_SETUP=false'
  echo 'export PIB_REPO=kubernetes101/pib-dev'
  echo 'export PIB_BRANCH=kev-ms'
  echo 'export PIB_CLUSTER=outerloop-kms1'
  echo 'export PIB_ARC_ENABLED=false'
  echo 'export PIB_OSM_ENABLED=false'
  echo 'export PIB_DAPR_ENABLED=false'
  echo 'export PIB_RESOURCE_GROUP=pib-devbox-project-outerloop-test'
  echo 'export PIB_SSL='
  echo 'export PIB_DNS_RG='
  echo 'export PIB_KEYVAULT='
  echo ""
} > "/home/$PIB_ME/.zshenv"
# source the env vars
source "/home/$PIB_ME/.zshenv"
export DEBIAN_FRONTEND=noninteractive
export HOME=/root
cd /home/${PIB_ME} || exit
echo "$(date +'%Y-%m-%d %H:%M:%S')  starting" >> status
echo "${PIB_ME} ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers.d/pib
touch .sudo_as_admin_successful
cp /usr/share/zoneinfo/America/Chicago /etc/localtime
echo "$(date +'%Y-%m-%d %H:%M:%S')  upgrading" >> status
apt-get update
apt-get upgrade -y
# upgrade sshd security
{
  echo ""
  echo "ClientAliveInterval 120"
  echo "Port 2222"
  echo "Ciphers chacha20-poly1305@openssh.com,aes256-gcm@openssh.com,aes128-gcm@openssh.com,aes256-ctr,aes192-ctr,aes128-ctr"
} >> /etc/ssh/sshd_config
# restart sshd
systemctl restart sshd
echo "$(date +'%Y-%m-%d %H:%M:%S')  user config" >> status
# backup bashrc
cp /etc/bash.bashrc /etc/bash.bashrc.bak
# source .bashrc for non-interactive logins
sed -i "s/\[ -z \"\$PS1\" ] && return//" /etc/bash.bashrc
# make some directories we will need
mkdir -p "/home/${PIB_ME}/.ssh"
mkdir -p "/home/${PIB_ME}/.kube"
mkdir -p "/home/${PIB_ME}/bin"
mkdir -p "/home/${PIB_ME}/.local/bin"
mkdir -p "/home/${PIB_ME}/.k9s"
mkdir -p /root/.kube
# make some system dirs
mkdir -p /etc/docker
mkdir -p /prometheus
chown -R 65534:65534 /prometheus
mkdir -p /grafana
chown -R 472:0 /grafana
# create / add to groups
groupadd docker
usermod -aG sudo "${PIB_ME}"
usermod -aG admin "${PIB_ME}"
usermod -aG docker "${PIB_ME}"
gpasswd -a "${PIB_ME}" sudo
# change ownership
chown -R ${PIB_ME}:${PIB_ME} "/home/$PIB_ME"
echo "$(date +'%Y-%m-%d %H:%M:%S')  installing base" >> status
apt-get install -y zsh apt-utils dialog apt-transport-https ca-certificates
if [ "$PIB_PAT" = "" ]; then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  PIB_PAT not set" >> "/home/$PIB_ME/status"
  echo "PIB_PAT not set"
  exit 1
fi
echo "$(date +'%Y-%m-%d %H:%M:%S')  cloning GitHub repo" >> status
git clone "https://$PIB_PAT@github.com/$PIB_REPO" pib
# checkout the branch
if [ "$PIB_BRANCH" != "main" ]; then
  git -C pib checkout $PIB_BRANCH
fi
# change ownership
chown -R ${PIB_ME}:${PIB_ME} /home/${PIB_ME}
if [ ! -f pib/vm/setup/setup.sh ]; then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  setup.sh not found" >> status
  echo "setup.sh not found"
  exit 1
fi
# run the setup script as pib user
sudo -HEu pib pib/vm/setup/setup.sh
if [ "$?" != "0" ]; then
  echo "$(date +'%Y-%m-%d %H:%M:%S')  setup.sh failed" >> status
  echo "setup.sh failed"
  chown -R ${PIB_ME}:${PIB_ME} /home/${PIB_ME}
  exit 1
fi
