namespace LIkeFeature.Exceptions
{
    public class DuplicateLikeException : Exception
    {
        public DuplicateLikeException() : base("User has already liked this article") { }
    }

    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message) : base(message) { }
    }
}
