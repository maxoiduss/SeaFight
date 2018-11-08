using System;
using System.Collections;
using System.Collections.Generic;

using SeaFight.Enums;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Models
{
    public class Field : IEnumerable
    {
        (int X, int Y) Sizes;
        readonly ArrayList InternalList = new ArrayList();

        public static Field Empty = new Field(0, 0);

        public Field(int i, int j)
        {
            if (i > 0 && j > 0)
            { Sizes.X = i; Sizes.Y = j; }
            else
            { Sizes.X = Sizes.Y = 1; }

            BattleField = new FieldCell[Sizes.X, Sizes.Y];

            for (int x = 0; x < Sizes.X; ++x)
                for (int y = 0; y < Sizes.Y; ++y)
                {
                    BattleField[x, y] = new FieldCell();
                    InternalList.Add(BattleField[x, y]);
                }
        }

        public Field(FieldCell[,] field)
        {
            BattleField = field;
            Sizes.X = field.GetLength(0);
            Sizes.Y = field.GetLength(1);
        }

        public FieldCell this[int i, int j]
        {
            get => BattleField[i, j];
            set => BattleField[i, j] = value;
        }

        public FieldCell[] this[int i]
        {
            get => (FieldCell[])InternalList.GetRange(i * Sizes.Y, Sizes.Y).ToArray();
            set => RebuildContainers(i, value);
        }

        public int X { get => Sizes.X; }
        public int Y { get => Sizes.Y; }
        public int Size { get => Math.Min(Sizes.X, Sizes.Y); }
        public FieldCell[,] BattleField;

        public void Update((int, int) cellNo, FieldCell cell)
        {
            BattleField[cellNo.Item1, cellNo.Item2] = cell;
        }

        public void Update((int, int) cellNo, CellState state)
        {
            BattleField[cellNo.Item1, cellNo.Item2].State = state;
        }

        public void Update((int, int) cellNo, CellState state, bool isShip)
        {
            BattleField[cellNo.Item1, cellNo.Item2].State = state;
            BattleField[cellNo.Item1, cellNo.Item2].IsShipCell = isShip;
        }

        public void Update((int, int) cellNo, bool isShip)
        {
            BattleField[cellNo.Item1, cellNo.Item2].IsShipCell = isShip;
        }

        public bool Update((int, int) cellNo)
        {
            return BattleField[cellNo.Item1, cellNo.Item2].HandleStateIfClicked();
        }

        public void GenerateShips(int shipLength, Action<int, int> updateShipCellAction)
        {
            if (shipLength <= 0 || shipLength > Size)
            {
                ErrorDetected("Specified ship length is not valid for this battle field");
                return;
            }
            if (updateShipCellAction is null)
            {
                ErrorDetected($"Target action in {nameof(GenerateShips)}", ReasonType.NullError);
                return;
            }

            var x = new Random().Next(1, Size);
            var y = new Random().Next(1, Size);
            var direction = new Random().Next(0, 3);

            updateShipCellAction.Invoke(x - 1, y - 1);

            for (int k = 1; k <= shipLength; ++k)
            {
                switch (direction)
                {
                    case 0 when y == 0:
                        direction = 2;
                        y += k; --k;
                        continue;
                    case 1 when x == 0:
                        direction = 3;
                        x += k; --k;
                        continue;
                    case 2 when y == Size:
                        direction = 0;
                        y -= k; --k;
                        continue;
                    case 3 when x == Size:
                        direction = 1;
                        x -= k; --k;
                        continue;
                    case 0:
                        updateShipCellAction.Invoke(x - 1, y - 1); --y;
                        break;
                    case 1:
                        updateShipCellAction.Invoke(x - 1, y - 1); --x;
                        break;
                    case 2:
                        updateShipCellAction.Invoke(x - 1, y - 1); ++y;
                        break;
                    case 3:
                        updateShipCellAction.Invoke(x - 1, y - 1); ++x;
                        break;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        public List<(int, int)> GetIndexList()
        {
            var list = new List<(int, int)>();

            for (int x = 0; x < Sizes.X; ++x)
                for (int y = 0; y < Sizes.Y; ++y)
                    list.Add((x, y));
            return list;
        }

        public void RebuildBattleField(int row = -1)
        {
            if (row < 0)
            {
                for (int x = 0; x < Sizes.X; ++x)
                    for (int y = 0; y < Sizes.Y; ++y)
                    {
                        BattleField[x, y] = (FieldCell)InternalList[x * Sizes.Y + y];
                    }
            }
            else
            {
                for (int y = 0; y < Sizes.Y; ++y)
                    BattleField[row, y] = (FieldCell)InternalList[row * Sizes.Y + y];
            }
        }

        public void RebuildInternalList(int row = -1)
        {
            if (row < 0)
            {
                for (int x = 0; x < Sizes.X; ++x)
                    for (int y = 0; y < Sizes.Y; ++y)
                    {
                        InternalList[x * Sizes.Y + y] = BattleField[x, y];
                    }
            }
            else
            {
                for (int y = 0; y < Sizes.Y; ++y)
                    InternalList[row * Sizes.Y + y] = BattleField[row, y];
            }
        }

        protected void RebuildContainers(int row, FieldCell[] value)
        {
            for (int y = 0; y < Sizes.Y; ++y)
                InternalList[row * Sizes.Y + y] = BattleField[row, y] = value[y];
        }
    }
}
