#!/bin/bash

# this runs at Codespace creation - not part of pre-build

echo "post-create start"
echo "$(date +'%Y-%m-%d %H:%M:%S')    post-create start" >> "$HOME/status"

# add shared ssh key
if [ "$ID_RSA" != "" ] && [ "$ID_RSA_PUB" != "" ]
then
    echo "$ID_RSA" | base64 -d > "$HOME/.ssh/id_rsa"
    echo "$ID_RSA_PUB" | base64 -d > "$HOME/.ssh/id_rsa.pub"
    chmod 600 "$HOME"/.ssh/id*
fi

echo "update CLI"
.devcontainer/cli-update.sh

echo "update oh-my-zsh"
git -C "$HOME/.oh-my-zsh" pull

echo "post-create complete"
echo "$(date +'%Y-%m-%d %H:%M:%S')    post-create complete" >> "$HOME/status"
