using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using drift.Data;
using drift.Data.Entity;
using drift.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
                }

                return score;
            }
        }

        public void GenerateResults(int competitionId, string userId)
        {
            using (db)
            {
                Random random=new Random((int) (DateTime.Now.Ticks));
                var competition = getCompetition(competitionId, userId);
                if (competition.Finished)
                {
                    return;
                }

                var applications = _userService.GetApprovedApplicationsByCompetition(competitionId);
                var fakeCarName = "Participant #";
                var participantName = "Car #";
                for (int i = 0; i < applications.Count; i++)
                {
                    var result = new CompetitionResult()
                    {
                        CarNumber = applications[i].ParticipantNumber,
                        CompetitionId = competition.Id,
                        FirstPhaseScore = random.Next(1, 100),
                        FourthPhaseScore = random.Next(1, 100),
                        ParticipantCar = applications[i].CarModelAndName,
                        ParticipantId = applications[i].ApplicantId,
                        ParticipantName = applications[i].CarModelAndName,
                        SecondPhaseScore = random.Next(1, 100),
                        ThirdPhaseScore = random.Next(1, 100),
                    };
                    db.CompetitionResults.Add(result);
                }

                for (int i = 1; i <= ResultSize - applications.Count; i++)
                {
                    var result = new CompetitionResult()
                    {
                        CarNumber = i,
                        CompetitionId = competition.Id,
                        FirstPhaseScore = random.Next(1, 100),
                        FourthPhaseScore = random.Next(1, 100),
                        ParticipantCar = fakeCarName + i,
                        ParticipantId = userId,
                        ParticipantName = participantName + i,
                        SecondPhaseScore = random.Next(1, 100),
                        ThirdPhaseScore = random.Next(1, 100),
                    };
                    db.CompetitionResults.Add(result);
                }

                var presentResults = db.CompetitionResults.Where(cr => cr.CompetitionId == competitionId)
                    .Select(cr =>
                        new
                        {
                            firstPhase = cr.FirstPhaseScore,
                            secondPhase = cr.SecondPhaseScore,
                            thirdPhase = cr.ThirdPhaseScore,
                            fourthPhase = cr.FourthPhaseScore,
                            Id = cr.Id
                        }).ToList();

                competition.Finished = true;
                competition.RegistrationOpen = false;
                db.Competitions.Update(competition);
                db.SaveChanges();
                generateScores(competition);
            }
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

        public CompetitionBracket getResultsBracket(int competitionId)
        {
            using (db)
            {
                var bracket = new CompetitionBracket();
                var results = getResults(competitionId);
                bracket.SecondStageResults = new List<CompetitionResultDto>();
                bracket.FirstStageResults = results;
                for (int i = 0; i < results.Count; i += 2)
                {
                    bracket.SecondStageResults.Add(getWinnerFirstStage(results[i], results[i + 1]));
                }

                bracket.ThirdStageResults = new List<CompetitionResultDto>();
                bracket.FourthStageResults = new List<CompetitionResultDto>();
                for (int i = 0; i < bracket.SecondStageResults.Count; i += 2)
                {
                    Console.WriteLine(i);
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
                return bracket;
            }
        }

        private void updatePlaces(CompetitionBracket bracket)
        {
            var i = 1;
            var secondStagePlaces = bracket.SecondStageResults
                .OrderByDescending(dto => dto.FirstPhaseScore + dto.SecondPhaseScore)
                .ToList();
            var firstStagePlaces = bracket.FirstStageResults
                .OrderByDescending(dto => dto.FirstPhaseScore)
                .ToList();
            foreach (var result in bracket.FourthStageResults)
            {
                var resultDb = db.CompetitionResults.FirstOrDefault(res => res.Id == result.Id);
                resultDb.Place = i;
                calculateTotal(resultDb);
                i++;
                secondStagePlaces.Remove(result);
                firstStagePlaces.Remove(result);
            }

            Console.WriteLine();
            foreach (var result in secondStagePlaces)
            {
                var resultDb = db.CompetitionResults.FirstOrDefault(res => res.Id == result.Id);
                i = thirdStageRecalculation(resultDb, i);
                firstStagePlaces.Remove(result);
            }

            Console.WriteLine();
            foreach (var result in firstStagePlaces)
            {
                var resultDb = db.CompetitionResults.FirstOrDefault(res => res.Id == result.Id);
                firstStageRecalculation(resultDb, i);
                i++;
            }

            db.SaveChanges();
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
            return firstParticipant.FirstPhaseScore > secondParticipant.SecondPhaseScore
                ? firstParticipant
                : secondParticipant;
        }

        private CompetitionResultDto getWinnerThirdStage(CompetitionResultDto firstParticipant,
            CompetitionResultDto secondParticipant)
        {
            return firstParticipant.FirstPhaseScore > secondParticipant.ThirdPhaseScore
                ? firstParticipant
                : secondParticipant;
        }

        private CompetitionResultDto getLoserThirdStage(CompetitionResultDto firstParticipant,
            CompetitionResultDto secondParticipant)
        {
            return firstParticipant.FirstPhaseScore < secondParticipant.ThirdPhaseScore
                ? firstParticipant
                : secondParticipant;
        }

        public CompetitionResultDto storeResult(CompetitionResultDto dto)
        {
            using (db)
            {
                var result = db.CompetitionResults.Add(new CompetitionResult()
                {
                    CarNumber = dto.CarNumber,
                    CompetitionId = dto.Competition.Id,
                    ParticipantId = dto.User.Id,
                    FirstPhaseScore = dto.FirstPhaseScore,
                    SecondPhaseScore = dto.SecondPhaseScore,
                    ThirdPhaseScore = dto.ThirdPhaseScore,
                    FourthPhaseScore = dto.FourthPhaseScore,
                    Place = dto.Place,
                    TotalScore = dto.TotalScore,
                    ParticipantName = dto.ParticipantName
                });
                db.SaveChanges();
                dto.Id = result.Entity.Id;
            }

            return dto;
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