using DaybreakGames.Census.Stream;
using System.Collections.Generic;
using System.Linq;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class CensusStreamSubscriptionBuilder
    {
        private CensusStreamSubscription _subscription = new CensusStreamSubscription();
        //public CensusStreamSubscription Subscription => _subscription;

        public IEnumerable<string> Characters => _characters.ToArray();
        public IEnumerable<string> Worlds => _worlds.ToArray();
        public IEnumerable<string> EventNames => _eventNames.ToArray();

        private List<string> _characters = new List<string>();
        private List<string> _worlds = new List<string>();
        private List<string> _eventNames = new List<string>();

        /*
            public string Service { get; }
            public string Action { get; }
            public IEnumerable<string> Characters { get; set; }
            public IEnumerable<string> Worlds { get; set; }
            public IEnumerable<string> EventNames { get; set; }
         */

        public CensusStreamSubscriptionBuilder()
        {
            //_subscription = new CensusStreamSubscription();
        }

        public CensusStreamSubscription GetSubscription()
        {
            return _subscription;
        }

        public CensusStreamSubscription ClearSubscriptions()
        {
            _characters.Clear();
            _subscription.Characters = _characters;

            _worlds.Clear();
            _subscription.Worlds = _worlds;

            _eventNames.Clear();
            _subscription.EventNames = _eventNames;

            return _subscription;
        }

        public CensusStreamSubscription AddDeath()
        {
            _eventNames.Add("Death");
            _subscription.EventNames = _eventNames.ToArray();
            return _subscription;
        }

        public CensusStreamSubscription AddPlayerLogin()
        {
            _eventNames.Add("PlayerLogin");
            _subscription.EventNames = _eventNames;
            return _subscription;
        }

        public CensusStreamSubscription AddPlayerLogout()
        {
            _eventNames.Add("PlayerLogout");
            _subscription.EventNames = _eventNames;
            return _subscription;
        }

        public CensusStreamSubscription AddFacilityControl()
        {
            _eventNames.Add("FacilityControl");
            _subscription.EventNames = _eventNames;
            return _subscription;
        }

        public CensusStreamSubscription AddPlayerFacilityCapture()
        {
            _eventNames.Add("PlayerFacilityCapture");
            _subscription.EventNames = _eventNames;
            return _subscription;
        }


        public CensusStreamSubscription AddPlayerFacilityDefend()
        {
            _eventNames.Add("PlayerFacilityDefend");
            _subscription.EventNames = _eventNames;
            return _subscription;
        }

        public CensusStreamSubscription AddVehicleDestroy()
        {
            _eventNames.Add("VehicleDestroy");
            _subscription.EventNames = _eventNames;
            return _subscription;
        }

        // Add subscription to all Gain Experience events
        public CensusStreamSubscription AddGainExperience()
        {
            _eventNames.Add("GainExperience");
            _subscription.EventNames = _eventNames;
            return _subscription;
        }

        public CensusStreamSubscription AddGainExperienceId(int experienceId)
        {
            _eventNames.Add($"GainExperience_experience_id_{experienceId}");
            _subscription.EventNames = _eventNames;
            return _subscription;
        }

        public CensusStreamSubscription AddGainExperienceIds(IEnumerable<int> experienceIds)
        {
            var experienceEvents = experienceIds.Select(id => $"GainExperience_experience_id_{id}");
            _eventNames.AddRange(experienceEvents);
            _subscription.EventNames = _eventNames;
            return _subscription;
        }

        public CensusStreamSubscription AddWorld(int worldId)
        {
            _worlds.Add(worldId.ToString());
            _subscription.Worlds = _worlds;
            return _subscription;
        }

        public CensusStreamSubscription AddWorlds(IEnumerable<int> worldIds)
        {
            var experienceEvents = worldIds.Select(id => id.ToString());
            _worlds.AddRange(experienceEvents);
            _subscription.Worlds = _worlds;
            return _subscription;
        }

        public CensusStreamSubscription AddWorldsAll()
        {
            _worlds.Clear();
            _subscription.Worlds = new[] { "all" };
            return _subscription;
        }

        public CensusStreamSubscription AddCharacter(string characterId)
        {
            _characters.Add(characterId);
            _subscription.Characters = _characters;
            return _subscription;
        }

        public CensusStreamSubscription AddCharacters(IEnumerable<string> characterIds)
        {
            _characters.AddRange(characterIds);
            _subscription.Characters = _characters.ToArray();
            return _subscription;
        }

        public CensusStreamSubscription AddCharactersAll()
        {
            _characters.Clear();
            //_subscription.Characters = new[] { "all" };
            _subscription.Characters = new[] {
                    "5428011263382229073",
                    "5428407427900177825",
                    "5428213558401889681",
                    "5428823203443498065",
                    "5428432435527452977",
                    "5428257774265913857" }; //,
                    /*"5428569415116158529",
                    "5428021759056382561",
                    "5428168624786270673",
                    "5428130184556053057",
                    "5428072203500373873",
                    "5428662532303555105",
                    "5428186718632838273", //14
                    "5428916581495528097", "5428795397182982401", "5428094673264781489", "5428041429986337681", "5428726698613385809", //19
                    "5428465397111272801", "5428010917272300305", "5428176321548436065", "5428047126251381105", "5428851341656156337", //24
                    "5428407427900054177", "5428029729498611633", "5428117870033064785", "5428491127574494257", "5428010618042900833", //29
                    "5428010917263801233", "5428010618034947121", "5428130184533136977", "5428010618019917649", "5428010618034961505", //34
                    "5428010618040540177", "5428010618037379361", "5428010618042802369", "5428010618043177201", "5428016813492381873", //39
                    "5428010917256365553", "5428010618040130369", "5428010917270419953", "5428013610397684097", "5428013610413094481", //43
                    "5428229842967644097", "5428026242707987345", "5428041429983112449", "5428077644331007697", "5428077644847417953", //49
                    "5428130184556063937", "5428013610453168481", "5428163811549650017", "5428055175438872945", "5428209189755040593", //54
                    "5428222237686624529", "5428082727901561185", "5428092148918001985", "5428322870170045377", "5428322870170772529" };//, //59
                    //"5428392193616912993", "5428013610386410785", "5428035526964753137", "5428057349728198113", "5428285306555271249", "5428013610436556449", "5428310317310710209", "5428174054418841249", "5428011263382256961", "5428163811594387425", "5428612885789216273", "5428010917249789809", "5428482805475988321", "5428465397116922737", "5428105192788740273", "5428016813487227585", "5428213558386107569", "5428204126945513649", "5428491127516888785", "5428703501285792113", "5428614696951040609", "5428095401429783889", "5428123302617276929", "5428109895927798257", "5428209189756481473", "5428034441434233281", "5428748801586587153", "5428325880043939953", "5428491127581209057", "5428389656694174513", "5428010618040904337", "5428010917236404129", "5428191068385577025", "5428010618040371745", "5428011263382229073", "5428010917251219665", "5428010917249201697", "5428731025608574481", "5428559248112545553", "5428885884743372273", "5428510351685855377", "5428245075231810753", "5428848639778029409", "5428559248112328481", "5428064957366826801", "5428482805160352177", "5428037477599872049", "5428272618457394177", "5428551667103062881", "5428451988632764401", "5428631729625307345", "5428716652655166257", "5428153774131092353", "5428359100729973969", "5428179127156903281", "5428213558401589057", "5428010618042453585", "5428021759085083729" };
                    */
            return _subscription;
        }
    }
}
