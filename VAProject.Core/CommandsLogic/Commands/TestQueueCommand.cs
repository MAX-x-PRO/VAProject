using System;
using System.Collections.Generic;
using System.Text;
using VAProject.Core.Enums;
using VAProject.Core.Interfaces;
using VAProject.Core.Models.Notifications;
using VAProject.Core.Utils;

namespace VAProject.Core.CommandsLogic.Commands
{
    public class TestQueueCommand : IVoiceCommand
    {
        public List<string> Triggers => new List<string> 
        { 
            "test queue",
            "test que",
            "best que"
        };

        public Task<CommandResult> OnExecute(string text)
        {
            var testQueue = new BiDirectionalPriorityQueue<string>();

            testQueue.Enqueue("First priority", 1);
            Thread.Sleep(10);

            testQueue.Enqueue("Second priority", 5);
            Thread.Sleep(10);

            testQueue.Enqueue("Third priority", 10);
            Thread.Sleep(10);

            testQueue.Enqueue("Fourth priority", 1);

            string highest = testQueue.Dequeue(QueueStrategy.HighestPriority);
            string oldest = testQueue.Dequeue(QueueStrategy.Oldest);
            string newest = testQueue.Dequeue(QueueStrategy.Newest);
            string lowest = testQueue.Dequeue(QueueStrategy.LowestPriority);

            string resultText = $"Test Lab 4 successful!\n" +
                                $"Highest: {highest}\n" +
                                $"Oldest: {oldest}\n" +
                                $"Newest: {newest}\n" +
                                $"Lowest: {lowest}";

            return Task.FromResult(new CommandResult
            {
                Success = true,
                LogMessage = "Lab 4 tested successfully.",
                TTSResponse = "I have tested the priority queue. Please check the screen for the extraction order.",
                NotificationPayload = new TextPayload
                {
                    Text = resultText,
                    DurationMs = 5000
                }
            });
        }
    }
}
