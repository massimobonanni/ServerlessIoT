using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Api.V2010.Account;
using System;
using System.Collections.Generic;
using System.Text;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using TelemetryEntities.Models;

namespace TelemetryEntities.Activities
{
    public class TwilioActivities
    {

        [FunctionName(nameof(SendMessageToTwilio))]
        [return: TwilioSms(AccountSidSetting = "TwilioAccountSid", AuthTokenSetting = "TwilioAuthToken")]
        public CreateMessageOptions SendMessageToTwilio([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var data = context.GetInput<NotificationData>();

            log.LogInformation($"Sending message to : {data.NotificationNumber}");

            var message = new CreateMessageOptions(new PhoneNumber(data.NotificationNumber))
            {
                From = new PhoneNumber(Environment.GetEnvironmentVariable("TwilioFromNumber")),
                Body = data.CreateMessage()
            };

            return message;
        }
    }
}
