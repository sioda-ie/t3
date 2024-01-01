﻿namespace Operators.Lib.Utils
{
    public static class AudioAnalysisResult
    {
        public static ResultsForFrequencyBand Bass;
        public static ResultsForFrequencyBand HiHats;

        public struct ResultsForFrequencyBand
        {
            public int PeakCount;
            public float TimeSincePeak;
            public double AccumulatedEnergy;
        }
    }
}