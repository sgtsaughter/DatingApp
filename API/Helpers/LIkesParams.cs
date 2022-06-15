namespace API.Helpers
{
    public class LIkesParams: PaginationParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; }
    }
}
