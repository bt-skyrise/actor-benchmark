set -e

docker build -t abimgregistry.azurecr.io/dapr-sut:latest -f Dockerfile-DaprSut ../.
docker push abimgregistry.azurecr.io/dapr-sut:latest