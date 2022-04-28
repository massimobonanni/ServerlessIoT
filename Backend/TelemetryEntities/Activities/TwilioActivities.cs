using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Api.V2010.Account;
using System;
using System.Collections.Generic;
using System.Text;
using Twilio.Types;
using Microsoft.Extensions.Configuration;

namespace TelemetryEntities.Activities
{
    public class TwilioActivities
    {
        public class SmsData
        {
            public string Message { get; set; }
            public string Destination { get; set; }
        }

        [FunctionName(nameof(SendMessageToTwilio))]
        [return: TwilioSms(AccountSidSetting = "TwilioAccountSid", AuthTokenSetting = "TwilioAuthToken")]
        public CreateMessageOptions SendMessageToTwilio([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            SmsData data = context.GetInput<SmsData>();

            log.LogInformation($"Sending message to : {data.Destination}");

            var message = new CreateMessageOptions(new PhoneNumber(data.Destination))
            {
                From = new PhoneNumber(Environment.GetEnvironmentVariable("TwilioFromNumber")),
                Body = data.Message
            };

            return message;
        }
    }
}
