using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum Requests
    {
        SendStatuses,
        ReceiveStatuses,
        SendAll,
        ReceiveAll,
        SendRoles,
        ReceiveRoles,
        SendUsers,
        ReceiveUsers,
        SendUser,
        ReceiveUser,
        SendCards,
        ReceiveCards,
        ReceiveCard,
        RemoveCard
    }
}
