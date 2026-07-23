using Kootam.UserManagement.Abstractions;
using Serilog.Core;
using Serilog.Events;

namespace Kootam.Utilities.SerilogRegistration.Enrichers
{
    public class UserInfoEnricher(IUserInfoService userInfoService) : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            string userName = userInfoService.GetUsername();
            if (string.IsNullOrEmpty(userName))
                userName = "Unknown";
            string UserId = userInfoService.UserIdOrDefault();
            string UserIp = userInfoService.GetUserIp();
            string clientId = userInfoService.GetClaim("client_id");
            if (string.IsNullOrEmpty(clientId))
                clientId = "Unknown";


            var userNameProperty = propertyFactory.CreateProperty("UserName", userName);
            var userIdProperty = propertyFactory.CreateProperty("UserId", UserId);
            var userIpProperty = propertyFactory.CreateProperty("UserIp", UserIp);
            var clientIdProperty = propertyFactory.CreateProperty("ClientId", clientId);

            logEvent.AddPropertyIfAbsent(userNameProperty);
            logEvent.AddPropertyIfAbsent(userIdProperty);
            logEvent.AddPropertyIfAbsent(userIpProperty);
            logEvent.AddPropertyIfAbsent(clientIdProperty);
        }
    }
}
