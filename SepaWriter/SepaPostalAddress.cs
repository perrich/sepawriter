using System;
using System.Collections.Generic;
using System.Linq;

namespace SepaWriter
{
    public class SepaPostalAddress
    {
        private string dept;
        private string subDept;
        private string strtNm;
        private string bldgNb;
        private string pstCd;
        private string twnNm;
        private string ctrySubDvsn;
        private string ctry;
        private List<string> adrLine;

        public PostalAddressType? AddressType { get; set; }

        public string Dept 
        { 
            get { return dept; }
            set 
            { 
                if (value != null && value.Length > 70)
                    throw new SepaRuleException(string.Format("Invalid length of Dept \"{0}\", must be less than or equal to 70 characters.", value));

                dept = value;

            }
        }
        public string SubDept
        { 
            get { return subDept; }
            set 
            { 
                if (value != null && value.Length > 70)
                    throw new SepaRuleException(string.Format("Invalid length of SubDept \"{0}\", must be less than or equal to 70 characters.", value));

                subDept = value;
            }
        }
        public string StrtNm
        { 
            get { return strtNm; }
            set 
            { 
                if (value != null && value.Length > 70)
                    throw new SepaRuleException(string.Format("Invalid length of StrtNm \"{0}\", must be less than or equal to 70 characters.", value));

                strtNm = value;
            }
        }
        public string BldgNb
        { 
            get { return bldgNb; }
            set 
            { 
                if (value != null && value.Length > 16)
                    throw new SepaRuleException(string.Format("Invalid length of BldgNb \"{0}\", must be less than or equal to 16 characters.", value));

                bldgNb = value;
            }
        }
        public string PstCd
        { 
            get { return pstCd; }
            set 
            { 
                if (value != null && value.Length > 16)
                    throw new SepaRuleException(string.Format("Invalid length of PstCd \"{0}\", must be less than or equal to 16 characters.", value));

                pstCd = value;
            }
        }
        public string TwnNm
        { 
            get { return twnNm; }
            set 
            { 
                if (value != null && value.Length > 35)
                    throw new SepaRuleException(string.Format("Invalid length of TwnNm \"{0}\", must be less than or equal to 35 characters.", value));

                twnNm = value;
            }
        }
        public string CtrySubDvsn
        { 
            get { return ctrySubDvsn; }
            set 
            { 
                if (value != null && value.Length > 35)
                    throw new SepaRuleException(string.Format("Invalid length of CtrySubDvsn \"{0}\", must be less than or equal to 35 characters.", value));

                ctrySubDvsn = value;
            }
        }
        public string Ctry
        { 
            get { return ctry; }
            set 
            { 
                if (value != null && value.Length != 2)
                    throw new SepaRuleException(string.Format("Invalid length of Ctry \"{0}\", must be less a 2 character ISO country code.", value));

                ctry = value.ToUpper();
            }
        }
        public List<String> AdrLine
        { 
            get { return adrLine; }
            set 
            {
                if (value != null)
                    foreach (var line in value)
                        if (line == null || line.Length > 70)
                            throw new SepaRuleException(string.Format("AdrLine cannot contain null items or an item has an invalid length of \"{0}\", must be less than or equal to 70 characters.", line));

                if (value != null && value.Count > 7)
                    throw new SepaRuleException(string.Format("AdrLine cannot contain more than 7 items, contains \"{0}\".", value.Count));

                adrLine = value;
            }
        }

        public bool IsValid 
        {
            get
            {
                return (String.IsNullOrEmpty(Dept) || Dept.Length <= 70) &&
                       (String.IsNullOrEmpty(SubDept) || SubDept.Length <= 70) &&
                       (String.IsNullOrEmpty(StrtNm) || StrtNm.Length <= 70) &&
                       (String.IsNullOrEmpty(BldgNb) || BldgNb.Length <= 16) &&
                       (String.IsNullOrEmpty(PstCd) || PstCd.Length <= 16) &&
                       (String.IsNullOrEmpty(TwnNm) || TwnNm.Length <= 35) &&
                       (String.IsNullOrEmpty(CtrySubDvsn) || CtrySubDvsn.Length <= 35) &&
                       (String.IsNullOrEmpty(Ctry) || Ctry.Length == 2) &&
                       (AdrLine == null || AdrLine.All(x => x.Length <= 70));
            }
        }
    }
}