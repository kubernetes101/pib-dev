# this image is built and updated weekly
# https://github.com/cse-labs/codespaces-images

FROM ghcr.io/cse-labs/k3d:latest

# some images require specific values
ARG USERNAME=vscode

# update welcome message; chown home dir
RUN echo "$(date +'%Y-%m-%d %H:%M:%S')    docker build start" >> "/home/${USERNAME}/status" && \
    mkdir -p "/home/${USERNAME}/.config/vscode-dev-containers" && \
    mkdir -p /usr/local/etc/vscode-dev-containers && \
    echo "👋 Welcome to Pilot-in-a-Box!" > /usr/local/etc/vscode-dev-containers/first-run-notice.txt && \
    echo "" >> /usr/local/etc/vscode-dev-containers/first-run-notice.txt && \
    echo "🔍 To explore PiB, open the README.md file" >> /usr/local/etc/vscode-dev-containers/first-run-notice.txt && \
    echo "" >> /usr/local/etc/vscode-dev-containers/first-run-notice.txt && \
    echo "cat /usr/local/etc/vscode-dev-containers/first-run-notice.txt" >> /etc/zsh/zshrc && \
    touch "/home/${USERNAME}/.config/vscode-dev-containers/first-run-notice-already-displayed" && \
    echo "$(date +'%Y-%m-%d %H:%M:%S')    docker build complete" >> "/home/${USERNAME}/status" && \
    chown -R ${USERNAME}:${USERNAME} "/home/${USERNAME}"
