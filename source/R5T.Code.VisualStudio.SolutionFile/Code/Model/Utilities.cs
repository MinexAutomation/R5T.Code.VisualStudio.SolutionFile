using System;

using R5T.NetStandard;


namespace R5T.Code.VisualStudio.Model
{
    public static class Utilities
    {
        public static PreOrPostSolution ToPreOrPostSolution(string value)
        {
            switch(value)
            {
                case "preSolution":
                    return PreOrPostSolution.PreSolution;

                case "postSolution":
                    return PreOrPostSolution.PostSolution;

                default:
                    throw new Exception(EnumHelper.UnrecognizedEnumerationValueMessage<PreOrPostSolution>(value));
            }
        }

        public static string ToStringStandard(PreOrPostSolution preOrPostSolution)
        {
            switch(preOrPostSolution)
            {
                case PreOrPostSolution.PreSolution:
                    return "preSolution";

                case PreOrPostSolution.PostSolution:
                    return "postSolution";

                default:
                    throw new Exception(EnumHelper.UnexpectedEnumerationValueMessage(preOrPostSolution));
            }
        }
    }
}
