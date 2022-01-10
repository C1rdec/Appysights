# Summary
Appysights need a configuration file to know how to display and where to fetch the informations. <br/>
This configuration file is a json representation of [Configuration.cs](https://github.com/C1rdec/Appysights/blob/9aecadbce39a4ed8e1d490e532cc254e09b5aaed/src/Appysights/Models/Configuration.cs#L1-L24)<br/>

There's two main concept `Service` and `Application`
- Service
   - A Service is an entity with a name and a collection of `Application` *[Max 3]*
- Application
   - An `Application` is where you set the [ApplicationId and ApiKey](https://dev.applicationinsights.io/documentation/Authorization/API-key-and-App-ID) of the desired Application Insights.
```
{
    "Name": "",
    "ApplicationId": "",
    "ApiKey": ""
}
```
### Here's how the informations is displayed

![image](https://user-images.githubusercontent.com/5436436/148718320-3145e41e-dd81-4a36-977d-3c48ada407a7.png)


# Simple example
```
{
    "Services": [
        {
            "Name": "",
            "Applications": [
                {
                    "Name": "",
                    "ApplicationId": "",
                    "ApiKey": ""
                },
                ...
            ]
        },
        ...
    ],
    "Statusbar": {
        "Name": "",
        "ApplicationId": "",
        "ApiKey": ""
    }
}
```
