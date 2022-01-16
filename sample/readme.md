# AzureFunctions.Middleware.Sample

This sample application provides an example on implementing global exception handler for azure functions. Extend the sample application to address cross-cutting concerns of 
your application.


# Run project 

```cmd

dotnet run

```

# Swagger endpoint

![Swagger Localhost UI](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/y9h5w1955ehjhvb4e9ky.png)

# Swagger UI

![Swagger UI](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/jwips0ja45ags5igtany.png)

# Middleware demo 

## 1. HTTP trigger without any unhandled exception flow 

![Middleware demo](../docs/sample.gif)

## 2. HTTP trigger throws exception flow

![Middleware demo exception handling](../docs/sample-1.gif)

## 3. UseWhen (Authorization) flow

![Middleware demo authorization](../docs/sample-2.gif)