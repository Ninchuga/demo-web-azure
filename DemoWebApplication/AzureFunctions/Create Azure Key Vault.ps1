$RESOURCE_GROUP_NAME='practice'
$KEY_VAULT_NAME='myazurepracticekeyvault'
$LOCATION='eastus'

# create Key Vault
az keyvault create `
    -g $RESOURCE_GROUP_NAME `
    -n $KEY_VAULT_NAME `
    --location $LOCATION

# set your key
$SEND_GRID_API_KEY='SendGridApiKey'
az keyvault secret set `
    -n $SEND_GRID_API_KEY `
    --value 'your-send-grid-api-key-value' `
    --vault-name $KEY_VAULT_NAME