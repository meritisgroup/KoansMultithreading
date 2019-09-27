using System;
using KoansMultithreading.AboutTaskObject.NoChangeAllow;
using KoansMultithreading.AboutTaskObject.NoChangeAllow.Contract;

namespace KoansMultithreading.AboutTaskObject.CanBeChanged
{
    public class MessageQueue : IMessageAPI, IMessageSender
    {
        public event Action<Message> Received;

        public event Action<Exception, Message> OnError;

        public void Push(Message msg) => Received(msg);
    }
}