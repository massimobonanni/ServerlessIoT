# AppSettings configuration

The app settings looks like the following:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "TwilioAccountSid": "",
    "TwilioAuthToken": "",
    "TwilioFromNumber": "",
    "IotHubName": "",
    "IoTHubConnectionAppSetting": ""
  }
}
```

* `AzureWebJobsStorage` : the function app storage account connection string. Use `UseDevelopmentStorage=true` for local test.
* `TwilioAccountSid` : The Twilio Account SID used by the functions to notify temperature threshold with SMS. Read <a href="https://www.twilio.com/docs/sms/send-messages" target="_blank">this</a> to understand how create a Twilio Account.
* `TwilioAuthToken` : The Twilio Auth Token used by the functions to notify temperature threshold with SMS. Read <a href="https://www.twilio.com/docs/sms/send-messages" target="_blank">this</a> to understand how create a Twilio Account.
* `TwilioFromNumber` : The Twilio Virtual Phone Number used by the functions to notify temperature threshold with SMS. Read <a href="https://www.twilio.com/docs/sms/send-messages" target="_blank">this</a> to understand how create a Twilio Account.
* `IotHubName` : The Event Hub-compatible name value you can find in the built-in endpoints blade of the IoTHub in Azure portal.

![](Images/EventHubCompatibleName.jpg)

* `IoTHubConnectionAppSetting` : The Event Hub-compatible endpoint you can find in the built-in endpoints blade of the IoTHub in Azure portal.

![](Images/EventHubCompatibleEndpoint.jpg)
