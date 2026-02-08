namespace Core.Model
{
    public interface IEnPassantProvider
    {
        Position? EnPassantTarget { get; }
    }
}