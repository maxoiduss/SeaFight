namespace SeaFight.Models
{
    public struct Statuses
    {
        public const string StartGame = "Start game";
        public const string RestartGame = "Restart game";
        public const string StartBattle = "Start battle";
        public const string GameFinished = "Game finished: ";
        public const string GameFinishedLuck = GameFinished + YouWin + RestartGame;
        public const string GameFinishedUnLuck = GameFinished + YouLose + RestartGame;
        public const string YouWin = "You win! ";
        public const string YouLose = "You lose! ";
    }
}
