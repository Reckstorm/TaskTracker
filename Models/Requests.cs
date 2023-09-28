using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum Requests
    {
        SendRoles,
        SendStatuses,
        SendUsers,
        SendUser,
        SendCards,
        ReceiveStatus,
        ReceiveUser,
        ReceiveCard,
        RemoveStatus,
        RemoveUser,
        RemoveCard,
        SearchCard,
        ConnectionClose
    }
}
