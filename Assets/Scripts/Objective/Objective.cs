using System;

namespace Assets.Scripts.Objective
{
    internal class Objective : IComparable<Objective>
    {
        internal bool Complete { get; set; } = false;
        internal string Name { get; set; }
        internal string Description { get; set; }

        internal DateTime TimeCompleted { get; set; }

        internal int Step { get; set; }

        /// <summary>
        /// Compares the current Objective instance with another Objective and returns an integer that indicates their
        /// relative order based on the Step property.
        /// </summary>
        /// <param name="other">The Objective to compare with the current instance.</param>
        /// <returns>A value less than zero if the current instance precedes other; zero if they have the same Step value; a
        /// value greater than zero if the current instance follows other.</returns>
        public int CompareTo(Objective other)
        {
            if(Step == other.Step)
            {
                return 0;
            }
            else if(Step < other.Step)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
