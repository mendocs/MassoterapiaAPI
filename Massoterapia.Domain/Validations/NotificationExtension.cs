using System.Collections.Generic;
using Flunt.Notifications;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Validation;

namespace Massoterapia.Domain.Validations
{
    public static class NotificationExtension
    {
        public static string AllInvalidations(this IReadOnlyCollection<Notification>  notifications)
        {
            string retorns = "";
            foreach(Notification notification in notifications)
                retorns += $"{notification.Key} : {notification.Message}; ";

            return retorns;
        }        
    }
}