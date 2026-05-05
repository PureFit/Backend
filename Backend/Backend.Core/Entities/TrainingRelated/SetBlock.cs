using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Entities.TrainingRelated
{
    public class SetBlock
    {
        public Guid Id { get; set; }

        public List<ExerciseEntry> ExerciseEntries { get; set; }

        public int SetsCount { get; set; } // how many times the full block should be performed
        public int RestTimeAfterBlockDoneSeconds { get; set; }

        public TrainingSet TrainingSet { get; set; } = null!;
        public Guid TrainingSetId { get; set; }
    }
}
