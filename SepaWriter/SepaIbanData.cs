using System;

namespace SepaWriter
{
    public class SepaIbanData : ICloneable
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                const int allowedLength = 70;

                if (value != null && value.Length > allowedLength)
                {
                    _name = value.Substring(0, allowedLength);
                }
                else
                {
                    _name = value;
                }
            }
        }

        public string Bic { get; set; }

        public string Iban { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}