using System;
using System.Collections;
using System.Collections.Generic;

namespace SeaFight.Models
{
    public class Ship : IEnumerable
    {
        public (int, int) StartCellNo { get; private set; }
        public (int, int) EndCellNo { get; private set; }
        public List<FieldCell> Body { get; private set; }
        public List<(int i, int j)> BodyNo { get; private set; }
        public int Length { get; private set; }

        public Ship() { Init(); }
        public Ship(bool automat) { Init(automat); }
        public Ship((int, int) start, (int, int) end)
        { StartCellNo = start; EndCellNo = end; Init(true); }

        void Init(bool generateAutomatically = false)
        {
            Body = new List<FieldCell>();
            BodyNo = new List<(int i, int j)>();

            if (generateAutomatically)
            {
                (int, int) left_bottom, right_top;
                if (StartCellNo.Item1 <= EndCellNo.Item1 && StartCellNo.Item2 <= EndCellNo.Item2)
                { left_bottom = StartCellNo; right_top = EndCellNo; }
                else
                { left_bottom = EndCellNo; right_top = StartCellNo; }

                for (int i = left_bottom.Item1; i < right_top.Item1; ++i)
                    for (int j = left_bottom.Item2; j < right_top.Item2; ++j)
                    {
                        Body.Add(new FieldCell(true));
                        BodyNo.Add((i, j));
                    }
            }
            EvaluateLength();
        }

        void EvaluateLength()
        {
            Length = Math.Max(Math.Abs(EndCellNo.Item1 - StartCellNo.Item1 + 1),
                              Math.Abs(EndCellNo.Item2 - StartCellNo.Item2 + 1));
        }

        public void EvaluateMagnitudes()
        {
            var list = new List<(int i, int j)>(BodyNo);
            var cont = list.Count;
            if (cont <= 0) return;

            list.Sort(Comparer<(int, int)>.Create((x, y) => x.Item1 <= y.Item1 ? x.Item1 == y.Item1 ? 0 : -1 : 1));
            list.Sort(Comparer<(int, int)>.Create((x, y) => x.Item2 <= y.Item2 ? x.Item2 == y.Item2 ? 0 : -1 : 1));
            StartCellNo = list[0];
            EndCellNo = list[cont - 1];
            EvaluateLength();
        }

        public void AddCell(FieldCell cell, int i, int j)
        {
            if (!Body.Contains(cell))
            {
                cell.IsShipCell = true;
                Body.Add(cell);
            }
            if (!BodyNo.Contains((i, j)))
                BodyNo.Add((i, j));
        }

        public void ChangeCellState(FieldCell cell, Enums.CellState state)
        {
            Body.Find((_cell) => _cell == cell).State = state;
        }

        public int CellsRemaining()
        {
            var allIntact = Body.FindAll(cell => cell.State != Enums.CellState.ShipAttacked);
            return allIntact.Count;
        }

        public bool IsDestroyed()
        {
            return Body.TrueForAll(cell => cell.State == Enums.CellState.ShipAttacked);
        }

        public IEnumerator GetEnumerator()
        {
            return Body.GetEnumerator();
        }
    }
}
