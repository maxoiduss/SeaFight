using System;
using System.Collections.Generic;

using SeaFight.Enums;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Models
{
    public class TurnInfo
    {
        public (int, int) CurrentTurn { get; set; }
        public uint SessionId { get; set; }
        public int RemainingCells { get; set; }
        public string PlayerName { get; set; }

        public TurnInfo() { PlayerName = Guid.NewGuid().ToString(); }
        public TurnInfo(uint id, string name) : this() { SessionId = id; PlayerName = name; }

        public override bool Equals(object obj)
        {
            var other = obj as TurnInfo;
            if (other is null)
            {
                ErrorDetected($"Error in {nameof(Equals)}: obj is not {typeof(TurnInfo).Name} or obj", ReasonType.NullError);
                return false;
            }
            return SessionId == other.SessionId && RemainingCells == other.RemainingCells && PlayerName.Equals(other.PlayerName);
        }

        public override int GetHashCode()
        {
            var hashCode = 1740370913;
            hashCode = hashCode * -1521134295 + SessionId.GetHashCode();
            hashCode = hashCode * -1521134295 + RemainingCells.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PlayerName);
            return hashCode;
        }
    }
}
