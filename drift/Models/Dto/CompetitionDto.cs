using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;

namespace drift.Models.Dto
{
    public class CompetitionDto
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Начало")]
        public DateTime StartDate { get; set; }
        public string CreatedById { get; set; }
        [DisplayName("Организатор")] public String CreatorUserName { get; set; }
        [DisplayName("Название")]
        public string Name { get; set; }
        [DisplayName("Регистрация")]
        public bool RegistrationOpen { get; set; }
        [DisplayName("Завершено")]
        public bool Finished { get; set; }
        [DisplayName("Описание")]
        public string Description { get; set; }
        [DisplayName("Сумма оплаты")]
        public double Fee { get; set; }

        public bool Participating { get; set; }

        public bool HasStages { get; set; }

        public List<CompetitionApplicationDto> Applications { get; set; }

        public override string ToString()
        {
            return
                $"{nameof(Id)}: {Id}, {nameof(StartDate)}: {StartDate}, {nameof(CreatedById)}: {CreatedById}, {nameof(CreatorUserName)}: {CreatorUserName}, {nameof(Name)}: {Name}, {nameof(RegistrationOpen)}: {RegistrationOpen}, {nameof(Finished)}: {Finished}, {nameof(Description)}: {Description}, {nameof(Fee)}: {Fee}";
        }
    }
}