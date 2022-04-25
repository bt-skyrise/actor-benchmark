set -e

docker build -t abimgregistry.azurecr.io/akka-sut:latest -f Dockerfile-AkkaSut ../.
docker push abimgregistry.azurecr.io/akka-sut:latest