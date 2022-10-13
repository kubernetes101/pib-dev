# PiB Templates

This directory contains `Kubernetes deployment yaml` templates. These templates are used by the PiB apps so that Application Teams don't have to create custom deployment yaml for each application.

You can add additional templates and reference from your application `app.yaml` file.

`GitOps Automation` automatically substutes the `{{gitops.*}}` values from the application and cluster metadata. GitOps Automation uses `Kustomize` to deploy the templates.
