
# description of script

# exit when any command fails
set -e

# variables
# docker hub profile
PROFILE="chrwalte"
# project name
PROJECT="paste.bin.ingest"
# version of the project
VERSION=$(cat ../VERSION)build

# BUILD
# cmd
echo "[CMD]: docker build --no-cache -t $PROJECT.cmd:$VERSION -f ./Dockerfile.cmd .."
docker build --no-cache -t $PROJECT.cmd:$VERSION -f ../Dockerfile.cmd ..
# api
echo "[CMD]: docker build --no-cache -t $PROJECT.api:$VERSION -f ./Dockerfile.api .."
docker build --no-cache -t $PROJECT.api:$VERSION -f ../Dockerfile.api ..

# TAG
# cmd
echo "[CMD]: docker tag $PROJECT.cmd:$VERSION $PROFILE/$PROJECT.cmd:$VERSION"
docker tag $PROJECT.cmd:$VERSION $PROFILE/$PROJECT.cmd:$VERSION
echo "[CMD]: docker tag $PROJECT.cmd:$VERSION $PROFILE/$PROJECT.cmd:build"
docker tag $PROJECT.cmd:$VERSION $PROFILE/$PROJECT.cmd:build
# api
echo "[CMD]: docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:$VERSION"
docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:$VERSION
echo "[CMD]: docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:build"
docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:build

# PUSH
# cmd
echo "[CMD]: docker push $PROFILE/$PROJECT.cmd:$VERSION"
docker push $PROFILE/$PROJECT.cmd:$VERSION
docker push $PROFILE/$PROJECT.cmd:build
# api
echo "[CMD]: docker push $PROFILE/$PROJECT.api:$VERSION"
docker push $PROFILE/$PROJECT.api:$VERSION
docker push $PROFILE/$PROJECT.api:build

# finished
echo "[INFO]: FINISHED BUILD ($PROFILE/$PROJECT:$VERSION)!"