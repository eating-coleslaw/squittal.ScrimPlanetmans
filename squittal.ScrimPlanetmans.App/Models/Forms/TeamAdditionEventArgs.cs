using System;

namespace squittal.ScrimPlanetmans.Models.Forms
{
    public class TeamAdditionEventArgs : EventArgs
    {
        public string AdditionIdentifier { get; set; }
        public TeamAdditionInputType AdditionType { get; set; }

        public TeamAdditionEventArgs(TeamAdditionInputType additionType)
        {
            AdditionType = additionType;
        }

        public TeamAdditionEventArgs(string additionIdentifier, TeamAdditionInputType additionType)
        {
            AdditionIdentifier = additionIdentifier;
            AdditionType = additionType;
        }
    }
}
