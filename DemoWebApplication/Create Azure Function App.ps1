# 1 - you should log into azure first
az login

# 2 - if you have multiple subscriptions use
az account set -s "My Subscription Name"

$RESOURCE_GROUP_NAME='practice'
$LOCATION='westeurope'

# 3 - create storage account
$STORAGE_ACCOUNT='practice8b59'

az storage account create `
    -n $STORAGE_ACCOUNT `
    -l $LOCATION `
    -g $RESOURCE_GROUP_NAME

# 4 - create application insights
echo '{"Application_Type":"web"}' > props.json
$APP_INSIGHTS='mypracticefunctionsinsights'

az resource create `
    -g $RESOURCE_GROUP_NAME `
    -n $APP_INSIGHTS `
    --resource-type "Microsoft.Insights/components" `
    --properties "@props.json"

# 5 - create function app
$FUNCTION_APP_NAME='MyPracticeFunctions'

az functionapp create `
    -n $FUNCTION_APP_NAME `
    -g $RESOURCE_GROUP_NAME `
    --storage-account $STORAGE_ACCOUNT `
    --app-insights $APP_INSIGHTS `
    --consumption-plan-location $LOCATION `
    --runtime dotnet

# 6 - specify application settings. For example SendGrid Api Key
az functionapp config appsettings set `
    -n $FUNCTION_APP_NAME `
    -g $RESOURCE_GROUP_NAME `
    --settings "MySetting1=Hello" "MySetting2=World"

    


