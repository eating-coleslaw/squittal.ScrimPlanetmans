using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.CensusStream.Models
{
    public class GainExperiencePayload : PayloadBase, IEquitable<GainExperiencePayload>
    {
        public string CharacterId { get; set; }
        public int ExperienceId { get; set; }
        public int Amount { get; set; }
        public int? LoadoutId { get; set; }
        public string OtherId { get; set; }

        #region IEquitable
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GainExperiencePayload);
        }

        public bool Equals(GainExperiencePayload p)
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
                    && p.CharacterId == CharacterId
                    && p.ExperienceId == ExperienceId
                    && p.OtherId == OtherId);
        }

        public static bool operator ==(GainExperiencePayload lhs, GainExperiencePayload rhs)
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

        public static bool operator !=(GainExperiencePayload lhs, GainExperiencePayload rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            var id = $"t{Timestamp:yyyyMMddTHHmmss}a{CharacterId}e{ExperienceId}r{OtherId}";
            return id.GetHashCode();
        }
        #endregion IEquitable
    }
}
