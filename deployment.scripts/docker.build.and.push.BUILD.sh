
# description of script

# exit when any command fails
set -ev

# variables
# docker hub profile
PROFILE="chrwalte"
# project name
PROJECT="paste.bin.ingest"
# version of the project
VERSION=$(cat ../VERSION)build

# BUILD
# cmd
docker build --no-cache -t $PROJECT.cmd:$VERSION -f ../Dockerfile.cmd ..
# api
docker build --no-cache -t $PROJECT.api:$VERSION -f ../Dockerfile.api ..

# TAG
# cmd
docker tag $PROJECT.cmd:$VERSION $PROFILE/$PROJECT.cmd:$VERSION
docker tag $PROJECT.cmd:$VERSION $PROFILE/$PROJECT.cmd:build
# api
docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:$VERSION
docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:build

# PUSH
# cmd
docker push $PROFILE/$PROJECT.cmd:$VERSION
docker push $PROFILE/$PROJECT.cmd:build
# api
docker push $PROFILE/$PROJECT.api:$VERSION
docker push $PROFILE/$PROJECT.api:build

# docker build, tag, and push BUILD script finished
