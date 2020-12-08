namespace QueenOfHearts.ExecutionService.Handlers.Requests
{
    internal class GetCommandRequest : AuthorizedRequest
    {
        public string CommandName { get; set; }
    }
}