using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceModel;
using ServiceAssembly.Models;
using ServiceAssembly.Interfaces;

namespace Activator.Models
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    public class ChatService : IChat
    {
        Dictionary<Client, IChatCallback> clients =
                 new Dictionary<Client, IChatCallback>();

        List<Client> clientList = new List<Client>();

        public INewServiceCallback CurrentCallback
        {
            get
            {
                return OperationContext.Current.
                       GetCallbackChannel<IChatCallback>();
            }
        }

        object syncObj = new object();

        private bool SearchClientsByName(string name)
        {
            foreach (Client c in clients.Keys)
            {
                if (c.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
