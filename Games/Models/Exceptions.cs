namespace Games.Models
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
    public class CharacterException : Exception
    {
        public CharacterException(string message) : base(message) { }
    }

    public class BattleException : Exception
    {
        public BattleException(string message) : base(message) { }
    }
}
