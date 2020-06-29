namespace squittal.ScrimPlanetmans.CensusStream.Models
{
    public class PlayerLogoutPayload : PayloadBase, IEquitablePayload<PlayerLogoutPayload>
    {
        public string CharacterId { get; set; }


        #region IEquitable
        public override bool Equals(object obj)
        {
            return this.Equals(obj as PlayerLogoutPayload);
        }

        public bool Equals(PlayerLogoutPayload p)
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
                    && p.CharacterId == CharacterId);
        }

        public static bool operator ==(PlayerLogoutPayload lhs, PlayerLogoutPayload rhs)
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

        public static bool operator !=(PlayerLogoutPayload lhs, PlayerLogoutPayload rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            var id = $"t{Timestamp:yyyyMMddTHHmmss}c{CharacterId}";
            return id.GetHashCode();
        }
        #endregion IEquitable
    }
}
