using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using drift.Data;
using drift.Data.Entity;
using drift.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace drift.Service
{
    public class CompetitionResultService
    {
        private ApplicationDbContext db;
        private IMapper _mapper;
        private UserService _userService;
        private const int ResultSize = 16;
        private Random _random;

        public CompetitionResultService(UserService userService, IMapper mapper, ApplicationDbContext db)
        {
            _userService = userService;
            _mapper = mapper;
            this.db = db;
            _random = new Random();
        }

        private List<CompetitionResultDto> getResults(int competitionId)
        {
            var result = getResultsDb(competitionId);
            return result.Select(dto => dto).OrderBy(dto => dto.CarNumber).ToList();
        }

        private List<CompetitionResultDto> getOverallResults(int competitionId)
        {
            var result = getResultsDb(competitionId);
            return result.Select(dto => dto).OrderBy(dto => dto.Place).ToList();
        }

        private IQueryable<CompetitionScoreDto> getScoreDb(int competitionId)
        {
            var result = from cs in db.CompetitionScores
                join c in db.Competitions on cs.CompetitionId equals c.Id
                orderby cs.ParticipantName
                where cs.CompetitionId == competitionId
                select new CompetitionScoreDto()
                {
                    Id = cs.Id,
                    AngleScore = cs.AngleScore,
                    StyleScore = cs.StyleScore,
                    TrackScore = cs.TrackScore,
                    Attempt = cs.Attempt,
                    Competition = new CompetitionDto() {Id = cs.CompetitionId},
                    ParticipantName = cs.ParticipantName,
                    Participant = _userService.FindById(cs.ParticipantId),
                    Total = cs.AngleScore + cs.TrackScore + cs.StyleScore
                };
            return result;
        }


        private IQueryable<CompetitionResultDto> getResultsDb(int competitionId)
        {
            var result = from cr in db.CompetitionResults
                join c in db.Competitions on cr.CompetitionId equals c.Id
                where cr.CompetitionId == competitionId
                select new CompetitionResultDto()
                {
                    Id = cr.Id,
                    Competition = _mapper.Map<CompetitionDto>(c),
                    Place = cr.Place,
                    FirstPhaseScore = cr.FirstPhaseScore,
                    SecondPhaseScore = cr.SecondPhaseScore,
                    ThirdPhaseScore = cr.ThirdPhaseScore,
                    FourthPhaseScore = cr.FourthPhaseScore,
                    CarNumber = cr.CarNumber,
                    TotalScore = cr.TotalScore,
                    User = _userService.FindById(cr.ParticipantId),
                    ParticipantName = cr.ParticipantName,
                };
            return result;
        }

        public List<CompetitionScoreDto> GetScore(int comeptitionId)
        {
            using (db)
            {
                var score = getScoreDb(comeptitionId).ToList();
                foreach (var scoreDto in score)
                {
                    var participantScore = db.CompetitionScores
                        .Where(cs => cs.CompetitionId == scoreDto.Competition.Id
                                     && cs.ParticipantName == scoreDto.ParticipantName);

                    scoreDto.BestAngle = participantScore.Max(cs => cs.AngleScore);
                    scoreDto.BestTrack = participantScore.Max(cs => cs.TrackScore);
                    scoreDto.BestStyle = participantScore.Max(cs => cs.StyleScore);
                    scoreDto.BestTotal = participantScore.Max(cs => cs.Total);
                }

                return score.OrderByDescending(cs => cs.BestTotal).ToList();
            }
        }

        public List<CompetitionApplicationDto> OrderApplicationsByScore(List<CompetitionApplicationDto> applications,
            int competitionId)
        {
            var scores = getScoreDb(competitionId).ToList();

            var applicationsOrdered = scores.Join(applications,
                    s => s.Participant.Id,
                    a => a.Participant.Id,
                    (s, a) => new
                    {
                        Score = s,
                        Application = a
                    })
                .Where(sc =>
                    sc.Score.Total == scores
                        .Where(s => s.Participant.Id.Equals(sc.Score.Participant.Id))
                        .Max(sc => sc.Total))
                .OrderByDescending(sc => sc.Score.Total)
                .Select(sc => sc.Application)
                .ToList();

            return applicationsOrdered;
        }

        public void GenerateResults(int competitionId, string userId, bool autoGenerate)
        {
            Random random = new Random((int) (DateTime.Now.Ticks));
            var competition = getCompetition(competitionId, userId);
            if (competition.Finished)
            {
                return;
            }

            var applications = _userService.GetApprovedApplicationsByCompetition(competitionId);
            applications = OrderApplicationsByScore(applications, competitionId);
            var fakeCarName = "Participant #";
            var participantName = "Car #";
            for (int i = 0; i < applications.Count; i++)
            {
                var result = new CompetitionResult()
                {
                    CarNumber = applications[i].ParticipantNumber,
                    CompetitionId = competition.Id,
                    FirstPhaseScore = createScore(autoGenerate, random),
                    FourthPhaseScore = createScore(autoGenerate, random),
                    ParticipantCar = applications[i].CarModelAndName,
                    ParticipantId = applications[i].ApplicantId,
                    ParticipantName = applications[i].ParticipantName,
                    SecondPhaseScore = createScore(autoGenerate, random),
                    ThirdPhaseScore = createScore(autoGenerate, random),
                    ParticipantNumber = i,
                };
                db.CompetitionResults.Add(result);
            }

            for (int i = applications.Count; i < ResultSize; i++)
            {
                var result = new CompetitionResult()
                {
                    CarNumber = i,
                    CompetitionId = competition.Id,
                    FirstPhaseScore = createScore(autoGenerate, random),
                    FourthPhaseScore = createScore(autoGenerate, random),
                    ParticipantCar = fakeCarName + i,
                    ParticipantId = userId,
                    ParticipantName = participantName + i,
                    SecondPhaseScore = createScore(autoGenerate, random),
                    ThirdPhaseScore = createScore(autoGenerate, random),
                    ParticipantNumber = i,
                };
                db.CompetitionResults.Add(result);
            }

            competition.Finished = true;
            competition.RegistrationOpen = false;
            db.Competitions.Update(competition);
            db.SaveChanges();
        }

        private int createScore(bool auto, Random random)
        {
            return auto ? random.Next(1, 100) : -1;
        }

        public void generateScores(Competition competition)
        {
            var results = getResults(competition.Id);
            var resultsScore = new List<CompetitionScore>();
            foreach (var result in results)
            {
                for (int i = 1; i <= 3; i++)
                {
                    var score = new CompetitionScore()
                    {
                        AngleScore = _random.Next(1, 100),
                        StyleScore = _random.Next(1, 100),
                        TrackScore = _random.Next(1, 100),
                        Attempt = i,
                        CompetitionId = competition.Id,
                        ParticipantId = result.User.Id,
                        ParticipantName = result.ParticipantName
                    };
                    resultsScore.Add(score);
                }
            }

            db.CompetitionScores.AddRange(resultsScore);
            db.SaveChanges();
        }

        private Competition getCompetition(int competitionId, string userId)
        {
            var competition = getCompetitionDb(competitionId);

            if (competition.CreatedById != userId)
            {
                throw new Exception("You can't access that competition ");
            }

            return competition;
        }

        private Competition? getCompetitionDb(int competitionId)
        {
            var competition = db.Competitions.FirstOrDefault(c => c.Id == competitionId);
            if (competition == null)
            {
                throw new Exception("Competition not found with id " + competitionId);
            }

            return competition;
        }

        public AllStagesResultResponse GetAllStagesResults(int competitionId)
        {
            var competition = _userService.GetCompetition(competitionId);
            var name = competition.Name.Split(' ')[0];
            var results = from cr in db.CompetitionResults
                join c in db.Competitions on cr.CompetitionId equals c.Id
                where c.Name.StartsWith(name)
                select new CompetitionResultDto()
                {
                    Id = cr.Id,
                    Competition = _mapper.Map<CompetitionDto>(c),
                    Place = cr.Place,
                    FirstPhaseScore = cr.FirstPhaseScore,
                    SecondPhaseScore = cr.SecondPhaseScore,
                    ThirdPhaseScore = cr.ThirdPhaseScore,
                    FourthPhaseScore = cr.FourthPhaseScore,
                    CarNumber = cr.CarNumber,
                    TotalScore = cr.TotalScore,
                    ParticipantName = cr.ParticipantName,
                };
            var totalResults = results.ToList()
                .GroupBy(r => r.ParticipantName)
                .Select(r =>
                    new CompetitionResultDto()
                    {
                        ParticipantName = r.Key,
                        TotalScore = (int) r.Sum(cr =>
                            cr.FirstPhaseScore + cr.SecondPhaseScore + cr.ThirdPhaseScore + cr.FourthPhaseScore)
                    }
                )
                .OrderByDescending(r => r.TotalScore)
                .ToList();
            var competitions = results.Select(r => r.Competition).Distinct().ToList();
            return new AllStagesResultResponse() {Results = totalResults, CompetitionDtos = competitions};
        }

        public CompetitionBracket getResultsBracket(int competitionId)
        {
            using (db)
            {
                var bracket = new CompetitionBracket();
                var results = getResults(competitionId);
                bracket.SecondStageResults = new List<CompetitionResultDto>();
                bracket.FirstStageResults = results;
                if (results.Count(r => r.FirstPhaseScore <= 0) > 0)
                {
                    GenerateEmptyResult(bracket);

                    return bracket;
                }

                for (int i = 0; i < results.Count; i += 2)
                {
                    bracket.SecondStageResults.Add(getWinnerFirstStage(results[i], results[i + 1]));
                }

                bracket.ThirdStageResults = new List<CompetitionResultDto>();
                bracket.FourthStageResults = new List<CompetitionResultDto>();
                for (int i = 0; i < bracket.SecondStageResults.Count; i += 2)
                {
                    bracket.ThirdStageResults.Add(getWinnerSecondStage(bracket.SecondStageResults[i],
                        bracket.SecondStageResults[i + 1]));
                }

                for (int i = 0; i < bracket.ThirdStageResults.Count; i += 2)
                {
                    bracket.FourthStageResults.Add(getWinnerThirdStage(bracket.ThirdStageResults[i],
                        bracket.ThirdStageResults[i + 1]));
                }

                for (int i = 0; i < bracket.ThirdStageResults.Count; i += 2)
                {
                    bracket.FourthStageResults.Add(getLoserThirdStage(bracket.ThirdStageResults[i],
                        bracket.ThirdStageResults[i + 1]));
                }

                updatePlaces(bracket);

                bracket.OverallResults = getOverallResults(competitionId);
                bracket.ResultsSet = true;

                return bracket;
            }
        }

        private static void GenerateEmptyResult(CompetitionBracket bracket)
        {
            bracket.SecondStageResults = new List<CompetitionResultDto>();
            bracket.ThirdStageResults = new List<CompetitionResultDto>();
            bracket.FourthStageResults = new List<CompetitionResultDto>();

            foreach (var bracketFirstStageResult in bracket.FirstStageResults)
            {
                bracketFirstStageResult.FirstPhaseScore = null;
            }

            for (int i = 0; i < 8; i++)
            {
                bracket.SecondStageResults.Add(new CompetitionResultDto());
            }

            for (int i = 0; i < 4; i++)
            {
                bracket.ThirdStageResults.Add(new CompetitionResultDto());
            }

            for (int i = 0; i < 4; i++)
            {
                bracket.FourthStageResults.Add(new CompetitionResultDto());
            }
        }

        private void updatePlaces(CompetitionBracket bracket)
        {
            var i = 5;

            var secondStagePlaces = bracket.SecondStageResults
                .OrderByDescending(dto => dto.SecondPhaseScore)
                .ToList();
            var firstStagePlaces = bracket.FirstStageResults
                .OrderByDescending(dto => dto.FirstPhaseScore)
                .ToList();

            for (var j = 0; j < bracket.FourthStageResults.Count; j += 2)
            {
                UpdateFourthStageResults(bracket.FourthStageResults, j, firstStagePlaces, secondStagePlaces);
            }

            foreach (var result in secondStagePlaces)
            {
                var resultDb = db.CompetitionResults.FirstOrDefault(res => res.Id == result.Id);
                i = thirdStageRecalculation(resultDb, i);
                firstStagePlaces.Remove(result);
            }

            foreach (var result in firstStagePlaces)
            {
                var resultDb = db.CompetitionResults.FirstOrDefault(res => res.Id == result.Id);
                firstStageRecalculation(resultDb, i);
                i++;
            }

            db.SaveChanges();
        }

        private void UpdateFourthStageResults(List<CompetitionResultDto> results, int i,
            List<CompetitionResultDto> firstStagePlaces, List<CompetitionResultDto> secondStagePlaces)
        {
            var firstResultDb =
                db.CompetitionResults.FirstOrDefault(res => res.Id == results[i].Id);
            var secondResultDb =
                db.CompetitionResults.FirstOrDefault(res => res.Id == results[i + 1].Id);

            calculateTotal(firstResultDb);
            calculateTotal(secondResultDb);

            firstResultDb.Place = firstResultDb.FourthPhaseScore > secondResultDb.FourthPhaseScore ? i + 1 : i + 2;
            secondResultDb.Place = secondResultDb.FourthPhaseScore > firstResultDb.FourthPhaseScore ? i + 1 : i + 2;

            firstStagePlaces.Remove(results[i]);
            firstStagePlaces.Remove(results[i + 1]);


            secondStagePlaces.Remove(results[i]);
            secondStagePlaces.Remove(results[i + 1]);
        }

        private static int thirdStageRecalculation(CompetitionResult? resultDb, int i)
        {
            resultDb.Place = i;
            resultDb.FourthPhaseScore = 0;
            resultDb.ThirdPhaseScore = 0;
            calculateTotal(resultDb);
            i++;
            return i;
        }

        private static void firstStageRecalculation(CompetitionResult? resultDb, int i)
        {
            resultDb.Place = i;
            resultDb.FourthPhaseScore = 0;
            resultDb.ThirdPhaseScore = 0;
            resultDb.SecondPhaseScore = 0;
            calculateTotal(resultDb);
        }

        private static void calculateTotal(CompetitionResult? resultDb)
        {
            resultDb.TotalScore = resultDb.FirstPhaseScore + resultDb.SecondPhaseScore + resultDb.ThirdPhaseScore +
                                  resultDb.FourthPhaseScore;
        }

        private CompetitionResultDto getWinnerFirstStage(CompetitionResultDto firstParticipant,
            CompetitionResultDto secondParticipant)
        {
            return firstParticipant.FirstPhaseScore > secondParticipant.FirstPhaseScore
                ? firstParticipant
                : secondParticipant;
        }

        private CompetitionResultDto getWinnerSecondStage(CompetitionResultDto firstParticipant,
            CompetitionResultDto secondParticipant)
        {
            return firstParticipant.SecondPhaseScore > secondParticipant.SecondPhaseScore
                ? firstParticipant
                : secondParticipant;
        }

        private CompetitionResultDto getWinnerThirdStage(CompetitionResultDto firstParticipant,
            CompetitionResultDto secondParticipant)
        {
            return firstParticipant.ThirdPhaseScore > secondParticipant.ThirdPhaseScore
                ? firstParticipant
                : secondParticipant;
        }

        private CompetitionResultDto getLoserThirdStage(CompetitionResultDto firstParticipant,
            CompetitionResultDto secondParticipant)
        {
            return firstParticipant.ThirdPhaseScore < secondParticipant.ThirdPhaseScore
                ? firstParticipant
                : secondParticipant;
        }

        public void storeResult(SetResultsRequest request)
        {
            var results = db.CompetitionResults.Where(cr => cr.CompetitionId == request.competitionId)
                .Select(cr => cr).OrderBy(dto => dto.CarNumber).ToList();
            var bracket = JsonConvert.DeserializeObject<List<List<List<int?>>>>(request.bracket);
            List<CompetitionResult> secondStageResults = new List<CompetitionResult>();
            List<CompetitionResult> thirdsStageResults = new List<CompetitionResult>();
            List<CompetitionResult> fourthStageResults = new List<CompetitionResult>();


            for (var i = 0; i < 8; i++)
            {
                results[2 * i].FirstPhaseScore = (int) bracket[0][i][0];
                results[2 * i + 1].FirstPhaseScore = (int) bracket[0][i][1];

                if (results[2 * i].FirstPhaseScore > results[2 * i + 1].FirstPhaseScore)
                {
                    secondStageResults.Add(results[2 * i]);
                }
                else
                {
                    secondStageResults.Add(results[2 * i + 1]);
                }
            }

            for (var i = 0; i < 8; i += 2)
            {
                secondStageResults[i].SecondPhaseScore = (int) bracket[1][i / 2][0];
                secondStageResults[i + 1].SecondPhaseScore = (int) bracket[1][i / 2][1];
                if (secondStageResults[i].SecondPhaseScore > secondStageResults[i + 1].SecondPhaseScore)
                {
                    thirdsStageResults.Add(secondStageResults[i]);
                }
                else
                {
                    thirdsStageResults.Add(secondStageResults[i + 1]);
                }
            }

            List<CompetitionResult> losers = new List<CompetitionResult>();
            for (int i = 0; i < 4; i += 2)
            {
                Console.WriteLine("[" + (int) bracket[2][i / 2][0] + "," + (int) bracket[2][i / 2][1] + "]");
            }

            for (var i = 0; i < 4; i += 2)
            {
                thirdsStageResults[i].ThirdPhaseScore = (int) bracket[2][i / 2][0];
                thirdsStageResults[i + 1].ThirdPhaseScore = (int) bracket[2][i / 2][1];
                if (thirdsStageResults[i].ThirdPhaseScore > thirdsStageResults[i + 1].ThirdPhaseScore)
                {
                    fourthStageResults.Add(thirdsStageResults[i]);
                    losers.Add(thirdsStageResults[i + 1]);
                }
                else
                {
                    fourthStageResults.Add(thirdsStageResults[i + 1]);
                    losers.Add(thirdsStageResults[i]);
                }
            }

            fourthStageResults.AddRange(losers);
            for (var i = 0; i < 4; i += 2)
            {
                fourthStageResults[i].FourthPhaseScore = (int) bracket[3][i / 2][0];
                fourthStageResults[i + 1].FourthPhaseScore = (int) bracket[3][i / 2][1];
            }

            db.SaveChanges();
        }

        public CompetitionResultDto updateResult(CompetitionResultDto dto)
        {
            using (db)
            {
                var competitionResult = db.CompetitionResults.FirstOrDefault(cr => cr.Id == dto.Id);
                if (competitionResult != null)
                {
                    competitionResult.TotalScore = dto.TotalScore;
                    db.SaveChanges();
                }
            }

            return dto;
        }
    }
}