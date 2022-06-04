
# description of script

# exit when any command fails
set -ev

# variables
# docker hub profile
PROFILE="chrwalte"
# project name
PROJECT="paste.bin.ingest"
# version of the project
VERSION=$(cat ../VERSION)

# BUILD
# cmd
docker build --no-cache -t $PROJECT.cmd:$VERSION -f ../Dockerfile.cmd ..
# api
docker build --no-cache -t $PROJECT.api:$VERSION -f ../Dockerfile.api ..

# TAG
# cmd
docker tag $PROJECT.cmd:$VERSION $PROFILE/$PROJECT.cmd:$VERSION-forArm
docker tag $PROJECT.cmd:$VERSION $PROFILE/$PROJECT.cmd:$VERSION-build-forArm
docker tag $PROJECT.cmd:$VERSION $PROFILE/$PROJECT.cmd:build-forArm
docker tag $PROJECT.cmd:$VERSION $PROFILE/$PROJECT.cmd:latest-forArm
# api
docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:$VERSION-forArm
docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:$VERSION-build-forArm
docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:build-forArm
docker tag $PROJECT.api:$VERSION $PROFILE/$PROJECT.api:latest-forArm

# PUSH
# cmd
docker push $PROFILE/$PROJECT.cmd:$VERSION-forArm
docker push $PROFILE/$PROJECT.cmd:$VERSION-build-forArm
docker push $PROFILE/$PROJECT.cmd:build-forArm
docker push $PROFILE/$PROJECT.cmd:latest-forArm
# api
docker push $PROFILE/$PROJECT.api:$VERSION-forArm
docker push $PROFILE/$PROJECT.api:$VERSION-build-forArm
docker push $PROFILE/$PROJECT.api:build-forArm
docker push $PROFILE/$PROJECT.api:latest-forArm

# docker build, tag, and push RELEASE script finished
