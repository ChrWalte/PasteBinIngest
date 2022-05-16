
# description of script

# exit when any command fails
set -e

# variables
# docker hub profile
PROFILE="chrwalte"
# project name
PROJECT="paste.bin.ingest"
# version of the project
VERSION=$(cat ../VERSION)

# BUILD
# cli
echo "[CMD]: docker build --no-cache -t $PROJECT.cli:$VERSION -f ./Dockerfile.cli .."
docker build --no-cache -t $PROJECT.cli:$VERSION -f ../Dockerfile.cli ..
# api
echo "[CMD]: docker build --no-cache -t $PROJECT.api:$VERSION -f ./Dockerfile.api .."
docker build --no-cache -t $PROJECT.api:$VERSION -f ../Dockerfile.api ..

# TAG
# cli
echo "[CMD]: docker tag $PROJECT.cli:$VERSION $PROFILE/$PROJECT.cli:$VERSION"
docker tag $PROJECT.cli:$VERSION $PROFILE/$PROJECT.cli:$VERSION
echo "[CMD]: docker tag $PROJECT.cli:$VERSION $PROFILE/$PROJECT.cli:latest"
docker tag $PROJECT.cli:$VERSION $PROFILE/$PROJECT.cli:latest
# api
echo "[CMD]: docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:$VERSION"
docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:$VERSION
echo "[CMD]: docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:latest"
docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:latest

# PUSH
# cli
echo "[CMD]: docker push $PROFILE/$PROJECT.cli:$VERSION"
docker push $PROFILE/$PROJECT.cli:$VERSION
docker push $PROFILE/$PROJECT.cli:latest
# api
echo "[CMD]: docker push $PROFILE/$PROJECT.api:$VERSION"
docker push $PROFILE/$PROJECT.api:$VERSION
docker push $PROFILE/$PROJECT.api:latest

# finished
echo "[INFO]: FINISHED BUILD ($PROFILE/$PROJECT:$VERSION)!"