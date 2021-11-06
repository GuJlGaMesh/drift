using AutoMapper;
using drift.Data;
using drift.Data.Entity;
using drift.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace drift.Service
{
	public class CompetitionResultService
	{
		private ApplicationDbContext _context;
		private IMapper _mapper;
		private UserService _userService;
		private const int ResultSize = 16;
		private Random _random;

		public CompetitionResultService(UserService userService, IMapper mapper, ApplicationDbContext context)
		{
			_userService = userService;
			_mapper = mapper;
			_context = context;
			_random = new Random();
		}

		private List<CompetitionResultDto> GetResults(int competitionId)
		{
			var result = GetResultsDb(competitionId);
			return result.Select(dto => dto).OrderBy(dto => dto.CarNumber).ToList();
		}

		private List<CompetitionResultDto> GetOverallResults(int competitionId)
		{
			var result = GetResultsDb(competitionId);
			return result.Select(dto => dto).OrderBy(dto => dto.Place).ToList();
		}

		private IQueryable<CompetitionScoreDto> GetScoreDb(int competitionId)
		{
			var result = from cs in _context.CompetitionScores
						 join c in _context.Competitions on cs.CompetitionId equals c.Id
						 orderby cs.ParticipantName
						 where cs.CompetitionId == competitionId
						 select new CompetitionScoreDto()
						 {
							 Id = cs.Id,
							 AngleScore = cs.AngleScore,
							 StyleScore = cs.StyleScore,
							 TrackScore = cs.TrackScore,
							 Attempt = cs.Attempt,
							 Competition = new CompetitionDto() { Id = cs.CompetitionId },
							 ParticipantName = cs.ParticipantName,
							 Total = cs.AngleScore + cs.TrackScore + cs.StyleScore
						 };
			return result;
		}


		private IQueryable<CompetitionResultDto> GetResultsDb(int competitionId)
		{
			var result = from cr in _context.CompetitionResults
						 join c in _context.Competitions on cr.CompetitionId equals c.Id
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

		public List<CompetitionScoreDto> GetScore(int competitionId)
		{
			var score = GetScoreDb(competitionId).ToList();
			foreach (var scoreDto in score)
			{
				var participantScore = _context.CompetitionScores
					.Where(cs => cs.CompetitionId == scoreDto.Competition.Id
								 && cs.ParticipantName == scoreDto.ParticipantName);

				scoreDto.BestAngle = participantScore.Max(cs => cs.AngleScore);
				scoreDto.BestTrack = participantScore.Max(cs => cs.TrackScore);
				scoreDto.BestStyle = participantScore.Max(cs => cs.StyleScore);
			}

			return score;
		}

		public void GenerateResults(int competitionId, string userId)
		{
			var competition = GetCompetition(competitionId, userId);
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
					FirstPhaseScore = _random.Next(1, 100),
					FourthPhaseScore = _random.Next(1, 100),
					ParticipantCar = applications[i].CarModelAndName,
					ParticipantId = applications[i].ApplicantId,
					ParticipantName = applications[i].CarModelAndName,
					SecondPhaseScore = _random.Next(1, 100),
					ThirdPhaseScore = _random.Next(1, 100),
				};
				_context.CompetitionResults.Add(result);
			}

			for (int i = 1; i <= ResultSize - applications.Count; i++)
			{
				var result = new CompetitionResult()
				{
					CarNumber = i,
					CompetitionId = competition.Id,
					FirstPhaseScore = _random.Next(1, 100),
					FourthPhaseScore = _random.Next(1, 100),
					ParticipantCar = fakeCarName + i,
					ParticipantId = userId,
					ParticipantName = participantName + i,
					SecondPhaseScore = _random.Next(1, 100),
					ThirdPhaseScore = _random.Next(1, 100),
				};
				_context.CompetitionResults.Add(result);
			}

			competition.Finished = true;
			_context.Competitions.Update(competition);
			_context.SaveChanges();
			GenerateScores(competition);
		}

		public void GenerateScores(Competition competition)
		{
			var results = GetResults(competition.Id);
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

			_context.CompetitionScores.AddRange(resultsScore);
			_context.SaveChanges();
		}

		private Competition GetCompetition(int competitionId, string userId)
		{
			var competition = GetCompetitionDb(competitionId);

			if (competition.CreatedById != userId)
			{
				throw new Exception("You can't access that competition ");
			}

			return competition;
		}

		private Competition? GetCompetitionDb(int competitionId)
		{
			var competition = _context.Competitions.FirstOrDefault(c => c.Id == competitionId);
			if (competition == null)
			{
				throw new Exception("Competition not found with id " + competitionId);
			}

			return competition;
		}

		public CompetitionBracket GetResultsBracket(int competitionId)
		{
			var bracket = new CompetitionBracket();
			var results = GetResults(competitionId);
			bracket.SecondStageResults = new List<CompetitionResultDto>();
			bracket.FirstStageResults = results;
			for (int i = 0; i < results.Count; i += 2)
			{
				bracket.SecondStageResults.Add(GetWinnerFirstStage(results[i], results[i + 1]));
			}

			bracket.ThirdStageResults = new List<CompetitionResultDto>();
			bracket.FourthStageResults = new List<CompetitionResultDto>();
			for (int i = 0; i < bracket.SecondStageResults.Count; i += 2)
			{
				Console.WriteLine(i);
				bracket.ThirdStageResults.Add(GetWinnerSecondStage(bracket.SecondStageResults[i],
					bracket.SecondStageResults[i + 1]));
			}

			for (int i = 0; i < bracket.ThirdStageResults.Count; i += 2)
			{
				bracket.FourthStageResults.Add(GetWinnerThirdStage(bracket.ThirdStageResults[i],
					bracket.ThirdStageResults[i + 1]));
			}

			for (int i = 0; i < bracket.ThirdStageResults.Count; i += 2)
			{
				bracket.FourthStageResults.Add(GetLoserThirdStage(bracket.ThirdStageResults[i],
					bracket.ThirdStageResults[i + 1]));
			}

			UpdatePlaces(bracket);
			bracket.OverallResults = GetOverallResults(competitionId);
			return bracket;
		}

		private void UpdatePlaces(CompetitionBracket bracket)
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
				var resultDb = _context.CompetitionResults.FirstOrDefault(res => res.Id == result.Id);
				resultDb.Place = i;
				CalculateTotal(resultDb);
				i++;
				secondStagePlaces.Remove(result);
				firstStagePlaces.Remove(result);
			}

			Console.WriteLine();
			foreach (var result in secondStagePlaces)
			{
				var resultDb = _context.CompetitionResults.FirstOrDefault(res => res.Id == result.Id);
				i = ThirdStageRecalculation(resultDb, i);
				firstStagePlaces.Remove(result);
			}

			Console.WriteLine();
			foreach (var result in firstStagePlaces)
			{
				var resultDb = _context.CompetitionResults.FirstOrDefault(res => res.Id == result.Id);
				FirstStageRecalculation(resultDb, i);
				i++;
			}

			_context.SaveChanges();
		}

		private static int ThirdStageRecalculation(CompetitionResult? resultDb, int i)
		{
			resultDb.Place = i;
			resultDb.FourthPhaseScore = 0;
			resultDb.ThirdPhaseScore = 0;
			CalculateTotal(resultDb);
			i++;
			return i;
		}

		private static void FirstStageRecalculation(CompetitionResult? resultDb, int i)
		{
			resultDb.Place = i;
			resultDb.FourthPhaseScore = 0;
			resultDb.ThirdPhaseScore = 0;
			resultDb.SecondPhaseScore = 0;
			CalculateTotal(resultDb);
		}

		private static void CalculateTotal(CompetitionResult? resultDb)
		{
			resultDb.TotalScore = resultDb.FirstPhaseScore + resultDb.SecondPhaseScore + resultDb.ThirdPhaseScore +
								  resultDb.FourthPhaseScore;
		}

		private CompetitionResultDto GetWinnerFirstStage(CompetitionResultDto firstParticipant,
			CompetitionResultDto secondParticipant)
		{
			return firstParticipant.FirstPhaseScore > secondParticipant.FirstPhaseScore
				? firstParticipant
				: secondParticipant;
		}

		private CompetitionResultDto GetWinnerSecondStage(CompetitionResultDto firstParticipant,
			CompetitionResultDto secondParticipant)
		{
			return firstParticipant.FirstPhaseScore > secondParticipant.SecondPhaseScore
				? firstParticipant
				: secondParticipant;
		}

		private CompetitionResultDto GetWinnerThirdStage(CompetitionResultDto firstParticipant,
			CompetitionResultDto secondParticipant)
		{
			return firstParticipant.FirstPhaseScore > secondParticipant.ThirdPhaseScore
				? firstParticipant
				: secondParticipant;
		}

		private CompetitionResultDto GetLoserThirdStage(CompetitionResultDto firstParticipant,
			CompetitionResultDto secondParticipant)
		{
			return firstParticipant.FirstPhaseScore < secondParticipant.ThirdPhaseScore
				? firstParticipant
				: secondParticipant;
		}

		public CompetitionResultDto StoreResult(CompetitionResultDto dto)
		{
			using (_context)
			{
				var result = _context.CompetitionResults.Add(new CompetitionResult()
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
				_context.SaveChanges();
				dto.Id = result.Entity.Id;
			}

			return dto;
		}

		public CompetitionResultDto UpdateResult(CompetitionResultDto dto)
		{
			using (_context)
			{
				var competitionResult = _context.CompetitionResults.FirstOrDefault(cr => cr.Id == dto.Id);
				if (competitionResult != null)
				{
					competitionResult.TotalScore = dto.TotalScore;
					_context.SaveChanges();
				}
			}

			return dto;
		}
	}
}