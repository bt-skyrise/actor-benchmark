## Environment setup

### Local tools

1. Azure CLI
2. Docker Desktop
3. kubectx
4. helm

### Azure

(You can use ARM template in the [azure](azure) directory as a reference)

Create following Azure resources:
1. Azure Storage Account - connection string is required for Orleans clustering
2. Azure Service Bus (Standard) - used to for communication between Test Runner and Test Runner client
3. Azure Container Registry - it will store images
4. Azure Kubernetes Service - environment for running the tests
   1. 2 node system pool, B2ms, label: `test-role=management`
   2. 3 node user pool, D4, label: `test-role=sut`
   3. 3 node user pool, D4, label: `test-role=runner`
   4. No RBAC
   5. No Azure monitoring
   6. Be sure to connect it with ACR upon creation

Get credentials for AKS, e.g.

```shell
az aks list -o table

# Name    Location     ResourceGroup    KubernetesVersion    ProvisioningState    Fqdn
# ------  -----------  ---------------  -------------------  -------------------  ---------------------------------------------
# ab-k8s  northeurope  ActorBenchmark   1.22.6               Succeeded            ab-k8s-dns-9a630584.hcp.northeurope.azmk8s.io

az aks get-credentials -n ab-k8s -g ActorBenchmark
```

## Supporting services in k8s

### Seq
Used for central logging. 

```shell
kubectl create namespace seq
helm repo add datalust https://helm.datalust.co
helm repo update
helm install my-seq -f kubernetes/values-seq.yaml -n seq datalust/seq
```

You can connect to seq by forwarding a port, e.g.:
```shell
kubectl port-forward service/my-seq -n seq 5341:80
```

Then open http://localhost:5241

### Prometheus

```shell
kubeclt create namespace prometheus
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update
helm install -n prometheus -f kubernetes/values-prometheus.yaml my-prometheus prometheus-community/prometheus
```

### Grafana

```shell
kubectl create namespace grafana
helm repo add grafana https://grafana.github.io/helm-charts
helm repo update
helm install -n grafana -f kubernetes/values-grafana.yaml my-grafana grafana/grafana
```

You can connect to Grafana by forwarding a port, e.g.:

```shell
kubectl port-forward service/my-grafana -n grafana 3000:80
```

Then open http://localhost:3000

Add datasource of type Prometheus, pointing to http://my-prometheus-server.prometheus 
Then import dashboards from the repo's [grafana](grafana) directory.

## Deploy benchmark to k8s

This walkthrough assumes the container registry is named `abimgregistry.azurecr.io`. If you're using a different name, update the `docker/image-*.sh` and `kubernetes/deployment-*.yaml` scripts.


### Prepare the images

Log into the image registry on your machine, e.g.:

```shell
az acr list -o table

# NAME           RESOURCE GROUP    LOCATION     SKU    LOGIN SERVER              CREATION DATE         ADMIN ENABLED
# -------------  ----------------  -----------  -----  ------------------------  --------------------  ---------------
# abimgregistry  ActorBenchmark    northeurope  Basic  abimgregistry.azurecr.io  2022-03-25T07:55:04Z  False

az acr login -n abimgregistry 
```

Run these scripts to build and push images. 

```shell
cd docker
./image-orleans-sut.sh
./image-proto-actor-sut.sh
./image-akka-sut.sh
./image-test-runner.sh
```

### Prepare namespace 

```shell
kubectl create namespace benchmark
```

### Orleans tests

#### Orleans SUT

Replace Azure storage connection string in `kubernetes/deployment-orleans-sut.yaml`

```shell
kubectl apply -n benchmark -f kubernetes/deployment-orleans-sut.yaml 
```

#### Test runner - for Orleans test

*Note: Only one configuration of test runner can be deployed at a time.*

Replace Azure Service bus connection string and Azure storage connection string in `kubernetes/deployment-test-runner-orleans.yaml`

```shell
kubectl apply -n benchmark -f kubernetes/deployment-test-runner-orleans.yaml
```

### Proto.Actor tests

#### Proto.Actor SUT

```shell
kubectl apply -n benchmark -f kubernetes/deployment-proto-actor-sut.yaml 
```

#### Test runner - for Proto.Actor test

*Note: Only one configuration of test runner can be deployed at a time.*

Replace Azure Service bus connection string string in `kubernetes/deployment-test-runner-proto-actor.yaml`

```shell
kubectl apply -n benchmark -f kubernetes/deployment-test-runner-proto-actor.yaml
```

### Akka tests

#### Lighthouse

Akka.net Lighthouse is used as seed node provider.

```shell
kubectl apply -n benchmark -f kubernetes/statefulset-lighthouse.yml
```

#### Akka SUT

```shell
kubectl apply -n benchmark -f kubernetes/deployment-akka-sut.yaml
```

#### Test runner - for Akka test

*Note: Only one configuration of test runner can be deployed at a time.*

Replace Azure Service bus connection string in `kubernetes/deployment-test-runner-akka.yaml`

```shell
kubectl apply -n benchmark -f kubernetes/deployment-test-runner-akka.yaml
```
