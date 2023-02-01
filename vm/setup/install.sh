#!/bin/bash

# this script installs most of the components

set -e

echo "$(date +'%Y-%m-%d %H:%M:%S')  install start" >> "$HOME/status"

sudo chown -R "${PIB_ME}:${PIB_ME}" "$HOME"

echo "$(date +'%Y-%m-%d %H:%M:%S')  installing CLI" >> "$HOME/status"
echo "installing cli"

mkdir -p "$HOME/bin"

cd "$HOME" || exit

# set zsh as default shell
sudo sed -i "s|/bin/bash|/bin/zsh|g" /etc/passwd

# install oh-my-zsh
if [ ! -d "$HOME/.oh-my-zsh" ]; then
  git clone https://github.com/ohmyzsh/oh-my-zsh .oh-my-zsh
  cp ".oh-my-zsh/templates/zshrc.zsh-template" .zshrc

  {
    echo ""
    echo "compinit"
  } >> ".zshrc"
fi

# use latest release
tag=$(curl -s https://api.github.com/repos/kubernetes101/pib-dev/releases/latest | grep tag_name | cut -d '"' -f4)

cd "$HOME/bin" || exit

if [ "$PIB_IS_INNER_LOOP" = "true" ]; then
  # install kic
  wget -O kic.tar.gz "https://github.com/kubernetes101/pib-dev/releases/download/$tag/kic-$tag-linux-amd64.tar.gz"
  tar -xvzf kic.tar.gz
  rm kic.tar.gz

  # install flt
  wget -O flt.tar.gz "https://github.com/kubernetes101/pib-dev/releases/download/$tag/flt-$tag-linux-amd64.tar.gz"
  tar -xvzf flt.tar.gz
  rm flt.tar.gz
else
  wget -O vm-kic.tar.gz "https://github.com/kubernetes101/pib-dev/releases/download/$tag/vm-kic-$tag-linux-amd64.tar.gz"
  tar -xvzf vm-kic.tar.gz
  rm vm-kic.tar.gz
fi

cd "$OLDPWD" || exit

echo "$(date +'%Y-%m-%d %H:%M:%S')  installing libs" >> "$HOME/status"
sudo apt-get install -y net-tools software-properties-common libssl-dev libffi-dev python-dev build-essential lsb-release gnupg-agent

echo "$(date +'%Y-%m-%d %H:%M:%S')  installing utils" >> "$HOME/status"
sudo apt-get install -y zsh curl git wget nano jq zip unzip httpie
sudo apt-get install -y dnsutils coreutils gnupg2 make bash-completion gettext iputils-ping

# add Docker source
echo "$(date +'%Y-%m-%d %H:%M:%S')  adding docker source" >> "$HOME/status"
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key --keyring /etc/apt/trusted.gpg.d/docker.gpg add -
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"

# add kubenetes source
echo "$(date +'%Y-%m-%d %H:%M:%S')  adding kubernetes source" >> "$HOME/status"
curl -fsSL https://packages.cloud.google.com/apt/doc/apt-key.gpg | sudo apt-key add -
echo "deb https://apt.kubernetes.io/ kubernetes-xenial main" | sudo tee /etc/apt/sources.list.d/kubernetes.list

# set shell to zsh
sudo sed -i "s|/bin/bash|/bin/zsh|g" /etc/passwd

echo "$(date +'%Y-%m-%d %H:%M:%S')  updating sources" >> "$HOME/status"

# this is failing on large fleets - add one retry

set +e

if ! sudo apt-get update
then
    echo "$(date +'%Y-%m-%d %H:%M:%S')  updating sources (retry)" >> "$HOME/status"
    sleep 30
    set -e
    sudo apt-get update
fi

set -e

echo "$(date +'%Y-%m-%d %H:%M:%S')  installing docker" >> "$HOME/status"
sudo apt-get install -y docker-ce docker-ce-cli

echo "$(date +'%Y-%m-%d %H:%M:%S')  installing kubectl" >> "$HOME/status"
sudo apt-get install -y kubectl

echo "$(date +'%Y-%m-%d %H:%M:%S')  installing k3d" >> "$HOME/status"
wget -q -O - https://raw.githubusercontent.com/rancher/k3d/main/install.sh | TAG=v5.4.6 sudo bash

echo "$(date +'%Y-%m-%d %H:%M:%S')  installing flux" >> "$HOME/status"
curl -s https://fluxcd.io/install.sh | sudo bash

echo "$(date +'%Y-%m-%d %H:%M:%S')  installing k9s" >> "$HOME/status"
VERSION=$(curl -i https://github.com/derailed/k9s/releases/latest | grep "location: https://github.com/" | rev | cut -f 1 -d / | rev | sed 's/\r//')
wget "https://github.com/derailed/k9s/releases/download/${VERSION}/k9s_Linux_amd64.tar.gz"
sudo tar -zxvf k9s_Linux_amd64.tar.gz -C /usr/local/bin
rm -f k9s_Linux_amd64.tar.gz

mkdir -p "$HOME/.oh-my-zsh/completions"
flux completion zsh > "$HOME/.oh-my-zsh/completions/_flux"
k3d completion zsh > "$HOME/.oh-my-zsh/completions/_k3d"
kubectl completion zsh > "$HOME/.oh-my-zsh/completions/_kubectl"

"$HOME/bin/kic" completion zsh > "$HOME/.oh-my-zsh/completions/_kic"

if [ "$PIB_IS_INNER_LOOP" = "true" ]; then
  "$HOME/bin/flt" completion zsh > "$HOME/.oh-my-zsh/completions/_flt"
fi

# update .zshenv
{
  echo ""
  echo "export PATH=\$PATH:\$HOME/bin"
  echo ""
  echo "alias k='kubectl'"
  echo "alias kaf='kubectl apply -f'"
  echo "alias kak='kubectl apply -k'"
  echo "alias kdelf='kubectl delete -f'"
  echo "alias kj='kubectl exec -it jumpbox -- bash -l'"
  echo "alias kje='kubectl exec -it jumpbox -- '"
} >> "$HOME/.zshenv"

{
  echo ""
  echo "shopt -s expand_aliases"
  echo ""
  echo "source \$HOME/.zshenv"
} > "$HOME/.bashrc"

# configure git CLI
git config --global user.name pib-gitops
git config --global user.email pib-gitops@outlook.com
git config --global core.whitespace blank-at-eol,blank-at-eof,space-before-tab
git config --global pull.rebase false
git config --global init.defaultbranch main
git config --global fetch.prune true
git config --global core.pager more
git config --global credential.helper store

# upgrade Ubuntu
echo "$(date +'%Y-%m-%d %H:%M:%S')  upgrading" >> "$HOME/status"
# sudo apt-get update
# sudo apt-get upgrade -y
# sudo apt-get autoremove -y

echo "$(date +'%Y-%m-%d %H:%M:%S')  install complete" >> "$HOME/status"
