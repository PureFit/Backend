using Backend.Core.Enums;

namespace Backend.Core.Entities.TrainingRelated
{
    public class WeekPlan
    {
        public Guid Id { get; set; }
        public int NumberInPlan { get; set; }
        public string Description { get; set; } = null!;
        public WeekStatus WeekStatus { get; set; } // active, past, future.

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public List<ExternalTraining> ExternalTrainings { get; set; } = []; // connect to calendar? can be duplicated.
                                                                             // when user added judo in google calendar and also when creating plan adds this training.
                                                                             // Maybe then we need to check somehow, if this training is the same as in google.
                                                                             // Or maybe the logic is: when user creates a plan - he chooses how often he trains.
                                                                             // Depending on it we create a plan and when user adds new training to google and doesnt add it here - its his problem.
                                                                             // We will add a button that creates a Training entry which should change our plan on this week from now on.
                                                                             // And then we can add a logic that user could choose instance on google calendar to say to us that this is a training.
                                                                             // and we will add this to our db also as an external training. and depending on it change the training plan.

        public List<TrainingSession> InternalTrainings { get; set; } = []; // training from our system but not related to the plan.
                                                                           // Maybe add a feature of adding it to google calendar but then we need to synchronise it when show the calendar.

        public List<PlanTraining> PlanTrainings { get; set; } = [];

        public Guid AIPlanId { get; set; }
        public AIPlan AIPlan { get; set; } = null!;
    }
}
