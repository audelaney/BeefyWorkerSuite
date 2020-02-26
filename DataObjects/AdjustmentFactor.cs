using System;

namespace DataObjects
{
    public enum AdjustmentFactor
    {
        /// <summary>
        /// Favor more wild changes in the encoder arguments for each new attempt
        /// </summary>
        accuracy,
        /// <summary>
        /// Favor a more complex/difficult starting point and smaller increments of change
        /// </summary>
        precision
    }
}