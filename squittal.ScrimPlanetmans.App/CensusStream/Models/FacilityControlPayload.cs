namespace squittal.ScrimPlanetmans.CensusStream.Models
{
    public class FacilityControlPayload : PayloadBase, IEquitablePayload<FacilityControlPayload>
    {
        public int FacilityId { get; set; }
        public int NewFactionId { get; set; }
        public int OldFactionId { get; set; }
        public int DurationHeld { get; set; }
        public string OutfitId { get; set; }

        #region IEquitable
        public override bool Equals(object obj)
        {
            return this.Equals(obj as FacilityControlPayload);
        }

        public bool Equals(FacilityControlPayload p)
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
                    && p.FacilityId == FacilityId
                    && p.WorldId == WorldId);
        }

        public static bool operator ==(FacilityControlPayload lhs, FacilityControlPayload rhs)
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

        public static bool operator !=(FacilityControlPayload lhs, FacilityControlPayload rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            var id = $"t{Timestamp:yyyyMMddTHHmmss}f{FacilityId}w{WorldId}";
            return id.GetHashCode();
        }
        #endregion IEquitable
    }
}
