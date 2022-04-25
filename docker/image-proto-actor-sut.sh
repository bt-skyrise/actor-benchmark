set -e

docker build -t abimgregistry.azurecr.io/proto-actor-sut:latest -f Dockerfile-ProtoActorSut ../.
docker push abimgregistry.azurecr.io/proto-actor-sut:latest