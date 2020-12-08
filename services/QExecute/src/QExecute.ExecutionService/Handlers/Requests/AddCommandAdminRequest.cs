namespace QueenOfHearts.ExecutionService.Handlers.Requests
{
    internal class AddCommandAdminRequest : AuthorizedRequest
    {
        public string CommandName { get; set; }
        public string AdminApiKey { get; set; }
    }
}