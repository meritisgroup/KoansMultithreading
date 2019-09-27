using System;

namespace KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract
{
    public interface IMessageAPI
    {
        event Action<Message> Received;

        event Action<Exception, Message> OnError;
    }
}