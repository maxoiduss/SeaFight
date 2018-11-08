using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using SeaFight.Enums;
using SeaFight.Models;
using static SeaFight.Services.Delays;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Services
{
    public class StubGameplayService : Interfaces.IGameplayService
    {
        #region rest implementation

        const string Url = "some_url";
        readonly TimeSpan requestTimeout = TimeSpan.FromSeconds(delayS);
        readonly CancellationTokenSource cts = new CancellationTokenSource(delayS);

        async Task<T> GetAsync<T>(string url, CancellationToken token = default(CancellationToken))
            where T : TurnInfo
        {
            try
            {
                await Task.Delay(delayMs);
                await Task.Run(() => { /*Console.WriteLine("GetAsync");*/ });
                return (T)EnemyInfo;
            }
            catch (Exception ex) { ErrorDetected($" in {nameof(GetAsync)}: {ex.Message} - {ex.StackTrace}", ReasonType.Exception); }
            return (T)new object();
        }

        async Task<T> PostAsync<T>(string url, object data, CancellationToken token = default(CancellationToken))
            where T : TurnInfo
        {
            try
            {
                await Task.Delay(delayMs);
                await Task.Run(() => { /*Console.WriteLine("PostAsync");*/ });
                return (T)PlayerInfo;
            }
            catch (Exception ex) { ErrorDetected($" in {nameof(PostAsync)}: {ex.Message} - {ex.StackTrace}", ReasonType.Exception); }
            return (T)new object();
        }

        #endregion


        static object lockObject = new object();
        static StubGameplayService self;
        public static StubGameplayService Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (self is null)
                    {
                        self = new StubGameplayService();
                    }
                }
                return self;
            }
        }

        TurnInfo playerInfo;
        TurnInfo PlayerInfo
        {
            get => playerInfo;
            set
            {
                playerInfo = value;
                playerInfo.RemainingCells = CountRemainingShipCells(false);
            }
        }
        TurnInfo enemyInfo;
        TurnInfo EnemyInfo
        {
            get => enemyInfo;
            set
            {
                enemyInfo = value;
                enemyInfo.RemainingCells = CountRemainingShipCells(true);
            }
        }

        static StubGameplayService() { }
        private StubGameplayService() { }

        public void Init(uint sessionId, string playerName,
                         Action<int, int, bool, int> updateShipCellAction,
                         Action simulateEnemyTurn,
                         Func<bool> checkVictory)
        {
            PlayerInfo = new TurnInfo(sessionId, playerName);
            if (updateShipCellAction is null || simulateEnemyTurn is null || checkVictory is null)
                ErrorDetected($"Error in {nameof(StubGameplayService)}'s {nameof(Init)}:" +
                              $"{nameof(updateShipCellAction)} or {nameof(simulateEnemyTurn)} or {nameof(checkVictory)}", ReasonType.NullError);
            else UpdateShipCell = updateShipCellAction;

            SimulateEnemyTurn = simulateEnemyTurn ?? new Action(() => { });
            CheckVictory = checkVictory ?? new Func<bool>(() => { return false; });
            Task.Run(async ()=> await SetEnemyInfo());
        }


        #region Fields for ViewModel

        public List<Ship> Ships { get; set; }
        public List<Ship> ShipsEnemy { get; set; }
        public List<(int, int)> RemainingTurns { get; set; }

        Field fieldModel;
        Field fieldEnemyModel;

        Func<bool> CheckVictory;
        Action SimulateEnemyTurn;
        Action<int, int, bool, int> UpdateShipCell;

        public void InitField(ref Field obj, Field newValue, string name)
        {
            if (newValue is null)
                ErrorDetected($"Error in {nameof(StubGameplayService)}'s {nameof(InitField)}:" +
                              $"{nameof(newValue)}", ReasonType.NullError);
            else switch (name)
            {
                case nameof(fieldModel): obj = fieldModel = newValue; break;
                case nameof(fieldEnemyModel): obj = fieldEnemyModel = newValue; break;
                default: obj = fieldModel = fieldEnemyModel = newValue; break;
            }
        }

        #endregion


        #region Methods for ViewModel

        public async Task CreateShips(params uint[] shipsSpecs)
        {
            await Task.Delay(delayMs);
            try
            {
                foreach (var spec in shipsSpecs)
                {
                    Ships.Add(new Ship());
                    ShipsEnemy.Add(new Ship());
                    fieldEnemyModel.GenerateShips((int)spec, (i, j) => UpdateShipCell(i, j, true, Ships.Count - 1));
                    fieldModel.GenerateShips((int)spec, (i, j) => UpdateShipCell(i, j, false, ShipsEnemy.Count - 1));
                    Ships[Ships.Count - 1].EvaluateMagnitudes(); ShipsEnemy[ShipsEnemy.Count - 1].EvaluateMagnitudes();
                }
            }
            catch (Exception ex)
            { ErrorDetected($" in {nameof(CreateShips)}: {ex.Message} - {ex.StackTrace}", ReasonType.Exception); }
        }

        public int CountRemainingShipCells(bool forEnemy)
        {
            var count = 0;
            if (forEnemy)
                ShipsEnemy?.ForEach((ship) => count += ship.CellsRemaining());
            else
                Ships?.ForEach((ship) => count += ship.CellsRemaining());

            return count;
        }

        public bool AnyRemainingShipCells(bool forEnemy)
        {
            bool? result;
            if (forEnemy)
                result = ShipsEnemy?.TrueForAll((cell) => !cell.IsDestroyed());
            else
                result = Ships?.TrueForAll((cell) => !cell.IsDestroyed());

            if (result.HasValue) return result.Value;

            return false;
        }

        #endregion


        public bool GameFinished
        {
            get => (PlayerInfo.RemainingCells != 0 && EnemyInfo.RemainingCells == 0)
            || (PlayerInfo.RemainingCells == 0 && EnemyInfo.RemainingCells != 0);
        }

        public bool Victory
        {
            get => PlayerInfo.RemainingCells != 0 && EnemyInfo.RemainingCells == 0;
        }

        public string PlayerName
        {
            get => PlayerInfo.PlayerName;
        }

        public string EnemyName
        {
            get => EnemyInfo?.PlayerName;
        }

        public uint SessionId
        {
            get => PlayerInfo.SessionId;
        }

        public int RemainingCells
        {
            get => PlayerInfo.RemainingCells;
        }

        public int RemainingEnemyCells
        {
            get => EnemyInfo.RemainingCells;
        }

        public async Task SetEnemyInfo(TurnInfo newInfo = null, bool newName = false, int withDelayMs = delayMs)
        {
            await Task.Delay(withDelayMs);

            var info = newInfo ?? EnemyInfo;
            if (newName)
                info.PlayerName = Guid.NewGuid().ToString();
            
            EnemyInfo = info ?? new TurnInfo { SessionId = SessionId };
        }

        public async Task GetEnemyInfo(bool simulating = false)
        {
            await Task.Delay(delayMs);

            var info = EnemyInfo;
            if (!(info is null) && !GameFinished)
            {
                var previousTurn = info.CurrentTurn;
                while (info.CurrentTurn.Item1 == previousTurn.Item1
                       && info.CurrentTurn.Item2 == previousTurn.Item2
                       && !GameFinished)
                {
                    if (simulating)
                    {
                        SimulateEnemyTurn?.Invoke();
                        break;
                    }
                    info = await GetAsync<TurnInfo>(Url, cts.Token);
                }
                await SetEnemyInfo(simulating ? null : info, withDelayMs: 0);
                await SetInfo(withDelayMs: 0);
            }
            CheckVictory?.Invoke();
        }

        public async Task SetEnemyTurn((int, int) coordinates, int withDelayMs = delayMs)
        {
            await Task.Delay(withDelayMs);

            var newInfo = EnemyInfo; newInfo.CurrentTurn = coordinates;
            await SetEnemyInfo(newInfo, withDelayMs: 0);
        }

        public async Task SetInfo(TurnInfo newInfo = null, int withDelayMs = delayMs)
        {
            await Task.Delay(withDelayMs);

            var info = newInfo ?? PlayerInfo;
            PlayerInfo = info ?? new TurnInfo { SessionId = SessionId };
        }

        public async Task Turn((int, int) coordinates)
        {
            await Task.Delay(delayMs);
            try
            {
                var info = PlayerInfo; info.CurrentTurn = coordinates;
                var newInfo = await PostAsync<TurnInfo>(Url, info, cts.Token);

                await SetInfo(newInfo, 0);
                await SetEnemyInfo(withDelayMs: 0);
                if (!((CheckVictory?.Invoke()).GetValueOrDefault()))
                    await GetEnemyInfo(true);
            }
            catch (Exception ex) { ErrorDetected($" in {nameof(Turn)}: {ex.Message} - {ex.StackTrace}", ReasonType.Exception); }
        }
    }
}
