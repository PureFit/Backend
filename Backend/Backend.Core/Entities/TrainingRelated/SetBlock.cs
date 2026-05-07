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

        public List<ExerciseEntry> ExerciseEntries { get; set; } = [];

        public int Order { get; set; }
        public int SetsCount { get; set; } // how many times the full block should be performed
        public int RestBetweenSetsSeconds { get; set; }   // rest between each set repetition
        public int RestAfterBlockSeconds { get; set; }    // rest after all sets of this block are done

        public TrainingSet TrainingSet { get; set; } = null!;
        public Guid TrainingSetId { get; set; }
    }
}
