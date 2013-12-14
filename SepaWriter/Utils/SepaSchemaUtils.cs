using System;

namespace Perrich.SepaWriter.Utils
{
    public static class SepaSchemaUtils
    {
        public static string SepaSchemaToString(SepaSchema schema)
        {
            switch (schema)
            {
                case SepaSchema.Pain00800102:
                    return "pain.008.001.02";
                case SepaSchema.Pain00800103:
                    return "pain.008.001.03";
                case SepaSchema.Pain00100103:
                    return "pain.001.001.03";
                case SepaSchema.Pain00100104:
                    return "pain.001.001.04";
                default:
                    throw new ArgumentException("unknown schema " + schema);
            }
        }
    }
}
