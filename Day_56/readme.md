
# Day 56

## Description 

### 1.Deployed via Direct Publish to Azure App Service
- Created a Web App in Azure App Service.

- Published the ASP.NET Core project directly from Visual Studio.

- Verified deployment by accessing the /weatherforecast endpoint.

### 2. Deployed via Azure Container Registry (ACR)
- Containerized the API using a custom Dockerfile.

-  Built and pushed the Docker image to Azure Container Registry (moumiacr.azurecr.io/mydockerapiapp:latest).

- Created a new App Service (Container type) and configured it to pull the image from ACR.

- Assigned a User Assigned Managed Identity (UAMI) to the App Service.

- Granted the AcrPull role to the identity for secure image access.

- Verified the container deployment via the public endpoint.

## Screenshots

### Direct Publish to App Service
![Uploaded Image](./outputs/app_service.png)

### Deployment Using ACR
![Deployment Using ACR](./outputs/using_acr.png)


