set -e

docker build -t abimgregistry.azurecr.io/orleans-sut:latest -f Dockerfile-OrleansSut ../.
docker push abimgregistry.azurecr.io/orleans-sut:latest