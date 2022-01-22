# Add a new deployment

Complete the following steps to add a new deployemnt:

## Step 1: Azure Subscription

Create a new Azure Subscription via the Portal. Name it: `SubscriptionGigLocal[Deployment]` e.g. `SubscriptionGigLocalProd`. Register the following Resource Providers:

- Microsoft.Web
- Microsoft.Sql
- Microsoft.Storage
- Microsoft.Cdn
- microsoft.insights

## Step 2: Resource Group

Create a Resource Group via the CLI. Name it: `GroupGigLocal[Deployment]` e.g. `GroupGigLocalProd`:

```
az group create --location australiaeast --name GroupGigLocal[Deployment] --subscription [subscription-id]
```

## Step 3: Service Principal

Create a Service Principal via the CLI. Name it: `ServicePrincipalGigLocal[Deployment]` e.g. `ServicePrincipalGigLocalProd`:

```
az ad sp create-for-rbac --name ServicePrincipalGigLocal[Deployment] --role contributor --scopes /subscriptions/[subscription-id]/resourceGroups/[resource-group-name] --sdk-auth
```

Save the output JSON as this is needed in the next step.

ND: CLI version must be higher than 2.25.0 to support AzAD breaking change.

## Step 4: GitHub Environment

Create a GitHub environment via github.com. Name it `[Deployment]` e.g. `Prod`. Add the following environment secrets:

- `AZURE_CREDENTIALS` - the output JSON from the Service Principal creation step
- `AZURE_RG` - the name of the Resource Group
- `AZURE_SQL_LOGIN` - a username for the SQL database e.g. `giglocaladmin`
- `AZURE_SQL_PASSWORD` - create a strong password for this
- `AZURE_SUBSCRIPTION` - the subscription id

## Step 5: GitHub Deployment

Add a new deployment section in the GitHub CI/CD workflow file for the new environment. Be sure to update the environment name according to the deployment name - it must only be 3 letters long.
