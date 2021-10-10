using System;

namespace drift.Models.Dto
{
    public class CompetitionDto
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }
        public string CreatedById { get; set; }
        public String CreatorUserName { get; set; }
        public string Name { get; set; }
        public bool RegistrationOpen { get; set; }
        public bool Finished { get; set; }
        public string Description { get; set; }
        public double Fee { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(StartDate)}: {StartDate}, {nameof(CreatedById)}: {CreatedById}, {nameof(CreatorUserName)}: {CreatorUserName}, {nameof(Name)}: {Name}, {nameof(RegistrationOpen)}: {RegistrationOpen}, {nameof(Finished)}: {Finished}, {nameof(Description)}: {Description}, {nameof(Fee)}: {Fee}";
        }
    }
}