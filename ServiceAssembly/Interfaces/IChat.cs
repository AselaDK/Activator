using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ServiceAssembly.Models;

namespace ServiceAssembly.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IChatCallback),
                         SessionMode = SessionMode.Required)]
    public interface IChat
    {
        [OperationContract(IsInitiating = true)]
        bool Connect(Client client);

        [OperationContract(IsOneWay = true)]
        void Say(Message msg);

        [OperationContract(IsOneWay = true)]
        void Whisper(Message msg, Client receiver);

        [OperationContract(IsOneWay = true)]
        void IsWriting(Client client);

        [OperationContract(IsOneWay = false)]
        bool SendFile(FileMessage fileMsg, Client receiver);

        [OperationContract(IsOneWay = true, IsTerminating = true)]
        void Disconnect(Client client);
    }

}
