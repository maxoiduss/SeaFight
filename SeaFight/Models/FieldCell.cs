using System;

using SeaFight.Enums;

namespace SeaFight.Models
{
    public class FieldCell
    {
        public bool IsShipCell { get; set; }

        public FieldCell() { State = CellState.Idle; }
        public FieldCell(CellState state) : this() { State = state; }
        public FieldCell(bool shipCell) : this() { IsShipCell = shipCell; }
        public FieldCell(CellState state, bool shipCell) : this(state) { IsShipCell = shipCell; }

        public CellState State { get; set; }

        public bool HandleStateIfClicked()
        {
            switch (State)
            {
                case CellState.Idle when IsShipCell: State = CellState.ShipAttacked;
                    break;
                case CellState.Idle: State = CellState.IdleOvercovered;
                    break;
                case CellState.ShipIdle: State = CellState.ShipAttacked;
                    break;
                case CellState.IdleOvercovered:
                case CellState.ShipAttacked:
                    return false;
            }
            return true;
        }
    }
}
