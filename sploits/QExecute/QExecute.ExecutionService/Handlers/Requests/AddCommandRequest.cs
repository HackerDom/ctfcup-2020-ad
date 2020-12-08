namespace QueenOfHearts.ExecutionService.Handlers.Requests
{
    internal class AddCommandRequest
    {
        public string ExecutorApiKey { get; set; }
        public string CommandName { get; set; }
    }
}