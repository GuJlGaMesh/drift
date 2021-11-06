using System;
using System.Collections.Generic;
using drift.Data.Entity;

namespace drift.Models.Dto
{
    public class CompetitionBracket
    {
        public List<CompetitionResultDto> FirstStageResults { get; set; }
        public List<CompetitionResultDto> SecondStageResults { get; set; }
        public List<CompetitionResultDto> ThirdStageResults { get; set; }
        public List<CompetitionResultDto> FourthStageResults { get; set; }
        
        public List<CompetitionResultDto> OverallResults { get; set; }


        public override string ToString()
        {
            return $"{nameof(FirstStageResults)}: {String.Join(";\n ", FirstStageResults)},\n" +
                   $" {nameof(SecondStageResults)}: {String.Join(";\n ", SecondStageResults)},\n" +
                   $" {nameof(ThirdStageResults)}: {String.Join(";\n ", ThirdStageResults)},\n" +
                   $" {nameof(FourthStageResults)}:  {String.Join(";\n ", FourthStageResults)}\n";
        }
    }
}