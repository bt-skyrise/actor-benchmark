set -e

docker build -t abimgregistry.azurecr.io/test-runner:latest -f Dockerfile-TestRunner ../.
docker push abimgregistry.azurecr.io/test-runner:latest