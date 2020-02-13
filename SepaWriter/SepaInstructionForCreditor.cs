namespace SepaWriter
{
    public class SepaInstructionForCreditor
    {
        public enum SepaInstructionForCreditorCode
        {
            CHQB,
            HOLD,
            PHOB,
            TELB,
        }

        public SepaInstructionForCreditorCode Code { get; set; }

        public string Comment { get; set; }
    }
}
