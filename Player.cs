namespace sokoban;

public class Player
{
    public string Name { get; }
    //public string Score { get; set; }

    // TODO сыграно игр, пройдено уровней, проведено времени в игре, короче статистика

    public Player(string name)
    {
        Name = name;
    }
}
