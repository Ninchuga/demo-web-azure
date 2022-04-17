# create new folder and navigate to it
md dockerfuncs
cd  dockerfuncs

# create new functions app and generate docker file
func init --docker

# list folder content
dir

# inspect created Dockerfile
cat .\Dockerfile

# create new function
func new --name HttpExample --template "HTTP trigger" --authlevel "anonymous"

# build image with docker build
docker build -t dockerfuncs:v1 .

# tag image
docker tag dockerfuncs:v1 ninchuga/myazurefunctionsrepo:dockerfuncsv1

# push image to docker hub, or alternatively to Azure ACR
docker push ninchuga/myazurefunctionsrepo:dockerfuncsv1