#!/bin/bash

tag=$1

echo "installing kic $tag"

mkdir -p "$HOME/bin"

# update CLI
cd "$HOME/bin" || exit

# remove old CLI
rm -rf kic .kic

# use latest release
if [ "$tag" = "" ]; then
    tag=$(curl -s https://api.github.com/repos/kubernetes101/pib-dev/releases/latest | grep tag_name | cut -d '"' -f4)
fi

# install kic
wget -O kic.tar.gz "https://github.com/kubernetes101/pib-dev/releases/download/$tag/kic-$tag-linux-amd64.tar.gz"
tar -xvzf kic.tar.gz
rm kic.tar.gz

# install flt
wget -O flt.tar.gz "https://github.com/kubernetes101/pib-dev/releases/download/$tag/flt-$tag-linux-amd64.tar.gz"
tar -xvzf flt.tar.gz
rm flt.tar.gz

echo "aka.ms/pib-cs-postsetup"
curl -i https://aka.ms/pib-cs-postsetup

cd "$OLDPWD" || exit
