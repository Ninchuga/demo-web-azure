$FUNCTION_APP_NAME='MyPracticeFunctions'
$RESOURCE_GROUP_NAME='practice'

# Publish Function App by using Command Line

# Method 1
func azure functionapp publish $FUNCTION_APP_NAME

# in the background zip file that was created was uploaded to a blob storage container in the storage account assocciated with our Function App,
# and then SAS Uri was generated that pointed to that zip file and new application settings were added to our function app that contain that SAS Uri
# Also, it doesn't have to be uploaded to a blob storage, it can be uploaded to a specific Function App Files Share
# You can find Files Shares by going to Advance Tools (Kudu) in Function App, it launches website known as Kudu which gives us lowlevel insights into the environment
# that is running our Function App. In Kudu click on Debug Console button then go to Data/Site Packages and there are your uploaded zip files
# zip file is unpacked in path /site/wwwroot

# list function appsettings
az functionapp config appsettings list `
    -g $RESOURCE_GROUP_NAME `
    -n $FUNCTION_APP_NAME `
    -o table

# Method 2
# build your project locally in release mode with command
dotnet publish -c release

    # this will create few files in .\bin\Release\net5.0\publish
    # we need to zip this folder *.zip

# then we can deploy our zip file by using command
az functionapp deployment source config-zip `
    -g $RESOURCE_GROUP_NAME `
    -n $FUNCTION_APP_NAME `
    --src publish.zip
