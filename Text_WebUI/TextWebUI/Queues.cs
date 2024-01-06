namespace Discord_AI_Presence.Text_WebUI.TextWebUI
{
    /// <summary>
    /// The AI may have many neuro net calls at the same time and it could possible lead to the neuro net getting overwhelmed.
    /// So we use a queue for these function calls.
    /// </summary>
    public class Queues
    {
        /// <summary>
        /// Gets the queue count.
        /// </summary>
        public int TotalInQueue => NeuroQueues.Count;
        private Queue<Func<Task>> NeuroQueues { get; set; } = [];
        private bool isProcessing = false;

        /// <summary>
        /// Queue an async Task that will be sent to retrieve data from the neuro net.
        /// </summary>
        /// <param name="neuroCall">The function</param>
        /// <returns></returns>
        public async Task Enqueue(Func<Task> neuroCall)
        {
            NeuroQueues.Enqueue(neuroCall);
            if (!isProcessing)
            {
                await Task.Run(ProcessQueue);
            }
        }

        private async Task ProcessQueue()
        {
            isProcessing = true;
            while (NeuroQueues.Count > 0)
            {
                var method = NeuroQueues.Dequeue();
                await method();
            }
            isProcessing = false;
        }
    }
}
