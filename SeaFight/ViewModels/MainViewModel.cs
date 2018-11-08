using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

using SeaFight.Enums;
using SeaFight.Models;
using SeaFight.Services;
using SeaFight.Services.Interfaces;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        IGameplayService Gameplay = StubGameplayService.Instance;

        string FieldTitleSuffix { get; set; } = "'s Field";
        string EnemyFieldTitleSuffix { get; set; } = "'s Field, your moves are here";
        string RemainingTitle { get; set; } = "Ship cells remain: ";
        string RemainingTitleEnemy { get; set; } = "Enemy ship cells remain: ";
        int MillisecondsCommonDelay { get; set; } = 0;
        int ShipLength { get; set; } = 4;
        int CellsCount { get; set; }


        int remainingShipCells;
        public int RemainingShipCells
        {
            get => remainingShipCells;
            set
            {
                SetProperty(ref remainingShipCells, value);
                RemainingShipCellsStr = RemainingTitle +
                    (remainingShipCells > 0 ? remainingShipCells.ToString() : string.Empty);
                OnPropertyChanged(nameof(RemainingShipCellsStr));
            }
        }
        public string RemainingShipCellsStr { get; protected set; }

        int remainingShipCellsEnemy;
        public int RemainingShipCellsEnemy
        {
            get => remainingShipCellsEnemy;
            set
            {
                SetProperty(ref remainingShipCellsEnemy, value);
                RemainingShipCellsEnemyStr = RemainingTitleEnemy +
                    (remainingShipCellsEnemy > 0 ? remainingShipCellsEnemy.ToString() : string.Empty);
                OnPropertyChanged(nameof(RemainingShipCellsEnemyStr));
            }
        }
        public string RemainingShipCellsEnemyStr { get; protected set; }

        TimeSpan sessionTime;
        public TimeSpan SessionTime
        {
            get => sessionTime;
            set
            {
                SetProperty(ref sessionTime, value);
                SessionTimeStr = SessionTime.TotalMilliseconds > 0 ? SessionTime.ToString() : string.Empty;
                OnPropertyChanged(nameof(SessionTimeStr));
            }
        }
        public string SessionTimeStr { get; protected set; }


        bool interactionsDisallowed;
        public bool InteractionsDisallowed
        {
            get => interactionsDisallowed;
            set => SetProperty(ref interactionsDisallowed, value);
        }

        string fieldTitle;
        public string FieldTitle
        {
            get => fieldTitle + FieldTitleSuffix;
            set => SetProperty(ref fieldTitle, value);
        }

        string enemyFieldTitle;
        public string EnemyFieldTitle
        {
            get => (string.IsNullOrWhiteSpace(enemyFieldTitle) ? "xxx" : enemyFieldTitle) + EnemyFieldTitleSuffix;
            set => SetProperty(ref enemyFieldTitle, value);
        }

        string gameStatus;
        public string GameStatus
        {
            get => gameStatus;
            set => SetProperty(ref gameStatus, value);
        }

        Field fieldModel;
        public Field FieldModel
        {
            get => fieldModel;
            set => SetProperty(ref fieldModel, value);
        }

        Field fieldEnemyModel;
        public Field FieldEnemyModel
        {
            get => fieldEnemyModel;
            set => SetProperty(ref fieldEnemyModel, value);
        }

        public FieldCell[,] BattleField
        {
            get => FieldModel.BattleField;
        }

        public FieldCell[,] BattleFieldEnemy
        {
            get => FieldEnemyModel.BattleField;
        }

        public MainViewModel(int scale, string playerName, string title = "SEA FIGHT")
        {
            Title = title;
            FieldTitle = playerName;

            Gameplay.Init((uint)new Random().Next(1, 1111111), FieldTitle, UpdateShipCell,
                          async () => await ExecuteWithDelay(() => EnemyTurn(null), true),
                          CheckVictory);

            GameStatus = Statuses.StartGame;
            CellsCount = scale > 0 ? scale : 1;
            RestartBattle();

            ClickCellCommand = new Command<(int, int)?>(async (cellNo) =>
                                                        await ExecuteWithDelay(()=> ClickCell(cellNo)));
            ClickCellEnemyCommand = new Command<(int, int)?>(async (_cellNo) =>
                                                             await ExecuteWithDelay(()=> EnemyTurn(_cellNo)));
            ControlFightCommand = new Command(async () => await ControlFight(), () => !IsBusy);
        }

        public ICommand ControlFightCommand { get; }
        public ICommand ClickCellCommand { get; }
        public ICommand ClickCellEnemyCommand { get; }

        void UpdateShipCell(int i, int j, bool forEnemy, int shipIndex)
        {
            if (forEnemy)
            {
                FieldEnemyModel.Update((i, j), true);
                Gameplay.ShipsEnemy[shipIndex].AddCell(FieldEnemyModel[i, j], i, j);
                OnPropertyChanged(nameof(BattleFieldEnemy));
            }
            else
            {
                FieldModel.Update((i, j), CellState.ShipIdle, true);
                Gameplay.Ships[shipIndex].AddCell(FieldModel[i, j], i, j);
                OnPropertyChanged(nameof(BattleField));
            }
        }

        bool CheckVictory()
        {
            if (Gameplay.GameFinished && Gameplay.Victory)
            {
                InteractionsDisallowed = true;
                GameStatus = Statuses.GameFinishedLuck; IsBusy = false;
                return true;
            }
            if (Gameplay.GameFinished && !Gameplay.Victory)
            {
                InteractionsDisallowed = true;
                GameStatus = Statuses.GameFinishedUnLuck; IsBusy = false;
                return true;
            }
            return false;
        }

        async Task ExecuteWithDelay(Action action, bool runAnyway = false)
        {
            if (IsBusy && !runAnyway) return;

            IsBusy = true;
            action?.Invoke();

            await Task.Run(() =>
            {
                RemainingShipCells = Gameplay.RemainingCells;
                RemainingShipCellsEnemy = Gameplay.RemainingEnemyCells;
            }).ContinueWith((_) => { return; });
            await Task.Delay(MillisecondsCommonDelay);

            IsBusy = false;
        }

        void EnemyTurn(object obj)
        {
            var cellNo = new Random().Next(0, Gameplay.RemainingTurns.Count - 1);
            var x = Gameplay.RemainingTurns[cellNo].Item1;
            var y = Gameplay.RemainingTurns[cellNo].Item2;
            FieldModel.Update((x, y));

            Gameplay.RemainingTurns.RemoveAt(cellNo);
            Gameplay.SetEnemyTurn((x, y));

            OnPropertyChanged(nameof(BattleField));
        }

        void ClickCell(object obj)
        {
            var cellNo = obj as (int, int)?;
            if (cellNo is null)
            {
                ErrorDetected($"Error in {nameof(ClickCell)}: cell is not a {typeof(Tuple<int, int>).Name}" +
                              $"or {nameof(obj)}", ReasonType.NullError);
                return;
            }

            if (CheckVictory()) return;
            if (FieldEnemyModel.Update(cellNo.Value))
            {
                Gameplay.Turn(cellNo.Value);
                OnPropertyChanged(nameof(BattleFieldEnemy));
            }
        }

        protected async Task ControlFight()
        {
            switch (GameStatus)
            {
                case Statuses.StartGame:
                    await Gameplay.CreateShips((uint)ShipLength);
                    GameStatus = Statuses.StartBattle;
                    break;
                case Statuses.StartBattle:
                    InteractionsDisallowed = false;
                    await Gameplay.SetInfo(withDelayMs: 0);
                    await Gameplay.SetEnemyInfo(newName: true, withDelayMs: 0);
                    EnemyFieldTitle = Gameplay.EnemyName.Substring(0, 10);
                    RestartTimer(); 
                    GameStatus = Statuses.RestartGame;
                    break;
                case Statuses.GameFinished:
                case Statuses.GameFinishedLuck:
                case Statuses.GameFinishedUnLuck:
                case Statuses.RestartGame:
                    RestartBattle(() => OnPropertiesChanged(
                        nameof(BattleField),
                        nameof(BattleFieldEnemy)));
                    GameStatus = Statuses.StartGame;
                    break;
            }
        }

        protected void RestartBattle(Action action = null)
        {
            InteractionsDisallowed = true;
            Gameplay.InitField(ref fieldModel, new Field(CellsCount, CellsCount), nameof(fieldModel));
            Gameplay.InitField(ref fieldEnemyModel, new Field(CellsCount, CellsCount), nameof(fieldEnemyModel));
            Gameplay.Ships = new List<Ship>();
            Gameplay.ShipsEnemy = new List<Ship>();
            Gameplay.RemainingTurns = FieldModel.GetIndexList();
            RemainingShipCells = 0;
            RemainingShipCellsEnemy = 0;

            action?.Invoke();
        }

        protected void RestartTimer(uint msStep = 100)
        {
            var msSpan = TimeSpan.FromMilliseconds(msStep);
            var now = DateTime.UtcNow;

            Device.StartTimer(msSpan, () =>
            {
                SessionTime = DateTime.UtcNow - now;
                return !InteractionsDisallowed;
            });
        }
    }
}
