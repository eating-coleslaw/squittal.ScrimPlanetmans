using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.CensusStream.Models
{
    public class DeathPayload : PayloadBase, IEquitable<DeathPayload>
    {
        public string AttackerCharacterId { get; set; }
        public int? AttackerFireModeId { get; set; }
        public int? AttackerLoadoutId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public int? AttackerWeaponId { get; set; }
        public string CharacterId { get; set; }
        public int? CharacterLoadoutId { get; set; }
        public bool IsHeadshot { get; set; }

        #region IEquitable
        public override bool Equals(object obj)
        {
            return this.Equals(obj as DeathPayload);
        }

        public bool Equals(DeathPayload p)
        {
            if (ReferenceEquals(p, null))
            {
                return false;
            }

            if (ReferenceEquals(this, p))
            {
                return true;
            }

            if (this.GetType() != p.GetType())
            {
                return false;
            }

            return (p.Timestamp == Timestamp
                    && p.AttackerCharacterId == AttackerCharacterId
                    && p.CharacterId == CharacterId);
        }

        public static bool operator ==(DeathPayload lhs, DeathPayload rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                if (ReferenceEquals(rhs, null))
                {
                    return true;
                }

                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(DeathPayload lhs, DeathPayload rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            var id = $"t{Timestamp:yyyyMMddTHHmmss}a{AttackerCharacterId}v{CharacterId}";
            return id.GetHashCode();
        }
        #endregion IEquitable
    }
}
