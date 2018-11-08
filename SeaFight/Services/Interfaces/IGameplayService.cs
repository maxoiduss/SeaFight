using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SeaFight.Models;
using static SeaFight.Services.Delays;

namespace SeaFight.Services.Interfaces
{
    public interface IGameplayService
    {
        string PlayerName { get; }
        string EnemyName { get; }
        uint SessionId { get; }

        bool Victory { get; }
        bool GameFinished { get; }
        int RemainingCells { get; }
        int RemainingEnemyCells { get; }

        Task Turn((int, int) coordinates);
        Task SetInfo(TurnInfo newInfo = null, int withDelayMs = delayMs);
        Task GetEnemyInfo(bool simulating = false);
        Task SetEnemyTurn((int, int) coordinates, int withDelayMs = delayMs);
        Task SetEnemyInfo(TurnInfo newInfo = null,
                          bool newName = false, int withDelayMs = delayMs);
        
        void Init(uint sessionId,
                  string playerName,
                  //int: x, int: y, bool: forEnemy?, int: shipIndex
                  Action<int, int, bool, int> updateShipCellAction,
                  Action simulateEnemyTurn,
                  Func<bool> checkVictory);

        List<Ship> Ships { get; set; }
        List<Ship> ShipsEnemy { get; set; }
        List<(int, int)> RemainingTurns { get; set; }

        Task CreateShips(params uint[] shipsSpecs);
        void InitField(ref Field obj, Field newValue, string name);

        bool AnyRemainingShipCells(bool forEnemy);
        int CountRemainingShipCells(bool forEnemy);
    }
}
