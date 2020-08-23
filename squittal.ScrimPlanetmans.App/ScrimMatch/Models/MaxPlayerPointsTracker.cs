using System.Threading;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class MaxPlayerPointsTracker
    {
        private int _points = 0;
        public string _owningCharacterId;

        private readonly object _objectLock = new object();

        private readonly AutoResetEvent _autoEvent = new AutoResetEvent(true);

        public bool TryUpdateMaxPoints(int points, string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                return false;
            }

            //lock(_objectLock)
            _autoEvent.WaitOne();
            //{
                if (string.IsNullOrWhiteSpace(_owningCharacterId))
                {
                    _points = points;
                    _owningCharacterId = characterId;

                    _autoEvent.Set();

                    return true;
                }

                if (points > _points)
                {
                    _points = points;
                    _owningCharacterId = characterId;

                    _autoEvent.Set();

                    return true;
                }
            //}

            _autoEvent.Set();
            return false;
        }

        public int GetMaxPoints()
        {
            return _points;
            //lock(_objectLock)
            //{
                //return _points;
            //}
        }

        public string GetOwningCharacterId()
        {
            return _owningCharacterId;
            //lock(_objectLock)
            //{
                //return _owningCharacterId;
            //}
        }
    }
}
