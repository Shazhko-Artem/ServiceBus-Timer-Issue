# Setup
You need to specify *serviceBus* connection string and *applicationInsights* connection string in the `appsettings.json` file.

# Reproduce the issue
You just need to launch the application and wait a couple of minutes. After that, go to the page of *applicationInsights* and see the logs. There you can see that the application sends a useless log every minute for each consumer.
 ![appInsight](/docs/appInsight.png)