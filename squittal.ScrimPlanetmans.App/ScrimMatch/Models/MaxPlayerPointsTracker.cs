namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class MaxPlayerPointsTracker
    {
        private int _points = 0;
        public string _owningCharacterId;

        private readonly object _objectLock = new object();

        public bool TryUpdateMaxPoints(int points, string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                return false;
            }
            
            lock(_objectLock)
            {
                if (string.IsNullOrWhiteSpace(_owningCharacterId))
                {
                    _points = points;
                    _owningCharacterId = characterId;
                    return true;
                }

                if (points > _points)
                {
                    _points = points;
                    _owningCharacterId = characterId;
                    return true;
                }
            }

            return false;
        }

        public int GetMaxPoints()
        {
            lock(_objectLock)
            {
                return _points;
            }
        }

        public string GetOwningCharacterId()
        {
            lock(_objectLock)
            {
                return _owningCharacterId;
            }
        }
    }
}
