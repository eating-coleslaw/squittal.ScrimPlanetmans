using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatchReports
{
    public class ScrimMatchReportDataService : IScrimMatchReportDataService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly ILogger<ScrimMatchReportDataService> _logger;

        private readonly int _scrimMatchBrowserPageSize = 20;

        public ScrimMatchReportDataService(IDbContextHelper dbContextHelper, ILogger<ScrimMatchReportDataService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _logger = logger;
        }

        public async Task<PaginatedList<ScrimMatchInfo>> GetHistoricalScrimMatchesListAsync(int? pageIndex)
        {
            /*
             SELECT config1.ScrimMatchId, MAX(StartTime), MAX(config1.ScrimMatchRound), MAX(config1.Title)
                FROM ScrimMatchRoundConfiguration config1
                INNER JOIN ScrimMatchRoundConfiguration config2
                    ON config1.ScrimMatchId = config2.ScrimMatchId
                INNER JOIN ScrimMatch match
                    ON config1.ScrimMatchId = match.Id 
                WHERE config1.ScrimMatchRound > config2.ScrimMatchRound
                GROUP BY config1.ScrimMatchId
                HAVING MAX(config1.ScrimMatchRound) > 1
                ORDER BY MAX(StartTime) DESC 
            */

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var scrimMatchesQuery = dbContext.ScrimMatchInfo.AsQueryable();

                //IQueryable<ScrimMatchInfo> scrimMatchesQuery =

                    //from matchInfo in dbContext.ScrimMatchInfo


                //IQueryable<ScrimMatchInfo> scrimMatchesQuery =

                //    from matchRound1 in dbContext.ScrimMatchRoundConfigurations

                //    join matchRound2 in dbContext.ScrimMatchRoundConfigurations
                //      on matchRound1.ScrimMatchId equals matchRound2.ScrimMatchId //into matchRoundsQ
                //    //from matchRounds in matchRoundsQ

                //    join world in dbContext.Worlds on matchRound1.WorldId equals world.Id into worldsQ
                //    from world in worldsQ

                //    join scrimMatch in dbContext.ScrimMatches on matchRound1.ScrimMatchId equals scrimMatch.Id into scrimMatchesQ
                //    from scrimMatch in scrimMatchesQ

                //    join facility in dbContext.MapRegions
                //        on matchRound1.FacilityId equals facility.FacilityId into facilitiesQ
                //    from facility in facilitiesQ

                //    where matchRound1.ScrimMatchRound > matchRound2.ScrimMatchRound //matchRounds.ScrimMatchRound
                //       && matchRound1.ScrimMatchRound > 1

                //    orderby scrimMatch.StartTime descending

                //    //group matchRound1 by matchRound1.ScrimMatchId into scrimMatchesGroup

                //    //select new ScrimMatchInfo()
                //    //{
                //    //    ScrimMatchId = scrimMatchesGroup.Key,
                //    //    StartTime = (from match in 
                //    //                 select match).Max(),
                //    //    Title = (from match in scrimMatchesGroup
                //    //             select match.Title).Max(),
                //    //    RoundCount = (from match in scrimMatchesGroup
                //    //                  select match.ScrimMatchRound).Max(),
                //    //    WorldId = (from match in scrimMatchesGroup
                //    //               select match.WorldId).Max(),
                //    //    WorldName = (from match in scrimMatchesGroup
                //    //                 select match.Name).Max(),
                //    //    FacilityId = (from match in scrimMatchesGroup
                //    //                  select match.FacilityId).Max(),
                //    //    FacilityName = (from match in scrimMatchesGroup
                //    //                    select match.FacilityName).Max(),
                //    //    EndRoundOnFacilityCapture = (from match in scrimMatchesGroup
                //    //                                 select match.IsRoundEndedOnFacilityCapture).Max()
                //    //};

                //    group new
                //    {
                //        matchRound1.ScrimMatchId,
                //        scrimMatch.StartTime,
                //        matchRound1.Title,
                //        matchRound1.ScrimMatchRound,
                //        matchRound1.WorldId,
                //        world.Name,
                //        matchRound1.FacilityId,
                //        facility.FacilityName,
                //        matchRound1.IsRoundEndedOnFacilityCapture
                //    }
                //    by matchRound1.ScrimMatchId into scrimMatchesGroup


                //    select new ScrimMatchInfo()
                //    {
                //        ScrimMatchId = scrimMatchesGroup.Key,
                //        //StartTime = (from match in scrimMatchesGroup
                //        //             select match.StartTime).Max(),
                //        Title = scrimMatchesGroup.Select(m => m.Title).First(),
                //        //Title = (from match in scrimMatchesGroup
                //                 //select match.Title).FirstOrDefault(),
                //        //RoundCount = (from match in scrimMatchesGroup
                //        //              select match.ScrimMatchRound).Max(),
                //        //WorldId = (from match in scrimMatchesGroup
                //        //           select match.WorldId).Max(),
                //        //WorldName = (from match in scrimMatchesGroup
                //        //             select match.Name).Max(),
                //        //FacilityId = (from match in scrimMatchesGroup
                //        //              select match.FacilityId).Max(),
                //        //FacilityName = (from match in scrimMatchesGroup
                //        //                select match.FacilityName).Max(),
                //        //EndRoundOnFacilityCapture = (from match in scrimMatchesGroup
                //        //                             select match.IsRoundEndedOnFacilityCapture).Max()
                //    };

                return await PaginatedList<ScrimMatchInfo>.CreateAsync(scrimMatchesQuery.AsNoTracking(), pageIndex ?? 1, _scrimMatchBrowserPageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                
                return null;
            }
        }
    }
}
