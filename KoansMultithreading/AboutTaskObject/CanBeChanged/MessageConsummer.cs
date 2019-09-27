using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using KoansMultithreading.AboutTaskObject.NoChangeAllow;

namespace KoansMultithreading.AboutTaskObject.CanBeChanged
{
    public class MessageConsummer
    {
        private readonly List<string> ReceivedOrder = new List<string>(10);
        private readonly List<string> ProcessOrder = new List<string>(10);
        private readonly List<string> ErrorMessage = new List<string>(10);
        private readonly List<Exception> Error = new List<Exception>(10);

        public async void Message_Received(Message msg)
        {
            ReceivedOrder.Add($"Received: {msg.Body}");
            await Task.Delay(1000);
            ProcessOrder.Add($"Processed: {msg.Body}");
        }

#pragma warning disable CS1998 // This async method don't have any 'await' so it wil be executed synchroniously
        public async void Message_Received_But_Exception_On_processing(Message msg)
#pragma warning restore CS1998 // This async method don't have any 'await' so it wil be executed synchroniously
        {
            throw new TimeoutException("Can not process message in time.");
        }

        public void ErrorHandler(Exception ex, Message msg)
        {
            ErrorMessage.Add($"Error on: {msg.Body} with error type: {ex.GetType().Name}");
            Error.Add(ex);
        }

        public void Foreach(MessageConsummerCase usecase, Action<string> testStrings)
        {
            var toBeProcessed = usecase == MessageConsummerCase.Received ? ReceivedOrder : ProcessOrder;
            foreach (var item in toBeProcessed)
            {
                testStrings(item);
            }
        }

        public void Foreach(Action<Exception> testStrings)
        {
            foreach (var item in Error)
            {
                testStrings(item);
            }
        }
    }
}