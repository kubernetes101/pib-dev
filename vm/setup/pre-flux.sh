#!/bin/bash

# this runs before flux-setup.sh

if [ "$PIB_IS_INNER_LOOP" = "true" ]; then
  exit 0
fi

echo "$(date +'%Y-%m-%d %H:%M:%S')  pre-flux start" >> "$HOME/status"

# change to this directory
cd "$(dirname "${BASH_SOURCE[0]}")" || exit

# create admin service account
kubectl create serviceaccount admin-user
kubectl create clusterrolebinding admin-user-binding --clusterrole cluster-admin --serviceaccount default:admin-user

kubectl apply -f - <<EOF
apiVersion: v1
kind: Secret
metadata:
  name: admin-user
  annotations:
    kubernetes.io/service-account.name: admin-user
type: kubernetes.io/service-account-token
EOF

# save Arc portal key
rm -f "$HOME/.ssh/arc.key"
kubectl get secret admin-user -o jsonpath='{$.data.token}' | base64 -d | sed $'s/$/\\\n/g' > "$HOME/.ssh/arc.key"
chmod 600 "$HOME/.ssh/arc.key"

if [ -f /home/pib/.ssh/fluent-bit.key ]
then
    kubectl create ns logging
    kubectl create secret generic fluent-bit-secrets -n logging --from-file "$HOME/.ssh/fluent-bit.key"
fi

if [ -f /home/pib/.ssh/prometheus.key ]
then
    kubectl create ns metrics
    kubectl create secret -n metrics generic prom-secrets --from-file "$HOME/.ssh/prometheus.key"
fi

if [ -n "$(find ./bootstrap/* -iregex '.*\.\(yaml\|yml\|json\)' 2>/dev/null)" ]
then
    kubectl apply -f ./bootstrap
    kubectl apply -R -f ./bootstrap
fi

echo "$(date +'%Y-%m-%d %H:%M:%S')  pre-flux complete" >> "$HOME/status"
