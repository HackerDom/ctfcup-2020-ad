namespace QueenOfHearts.ExecutionService.Handlers.Requests
{
    internal class JoinToCommandRequest : AuthorizedRequest
    {
        public string CommandName { get; set; }
    }
}