

namespace sssUmidSrvcs.Models
{
    public class CardData
    {

        public string CRN { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string suffix { get; set; }
        public string addressPostalCode { get; set; }
        public string addressCountry { get; set; }
        public string addressProvince { get; set; }
        public string addressCity { get; set; }
        public string addressBarangay { get; set; }
        public string addressSubdivision { get; set; }
        public string addressHouseLotBlockNo { get; set; }
        public string addressRmFlrUnitNoBldgName { get; set; }
        public string gender { get; set; }
        public string dateOfBirth { get; set; }
        public string leftPrimaryFingerCode { get; set; }
        public string rightPrimaryFingerCode { get; set; }
        public string leftBackupFingerCode { get; set; }
        public string rightBackupFingerCode { get; set; }
        public string leftPrimaryFingerprint { get; set; }
        public string rightPrimaryFingerprint { get; set; }
        public string leftBackupFingerprint { get; set; }
        public string rightBackupFingerprint { get; set; }
        public string picture { get; set; }
        public string height { get; set; }
        public string weight { get; set; }
        public string distinguishingFeatures { get; set; }
        public string placeOfBirthCity { get; set; }
        public string placeOfBirthProvince { get; set; }
        public string placeOfBirthCountry { get; set; }
        public string fatherFirstName { get; set; }
        public string fatherMiddleName { get; set; }
        public string fatherLastName { get; set; }
        public string fatherSuffix { get; set; }
        public string motherFirstName { get; set; }
        public string motherMiddleName { get; set; }
        public string motherLastName { get; set; }
        public string motherSuffix { get; set; }
        public string tin { get; set; }
        public string cardStatus { get; set; }

        public CardData()
        {
            CRN = "";
            firstName = "";
            middleName = "";
            lastName = "";
            suffix = "";
            addressPostalCode = "";
            addressCountry = "";
            addressProvince = "";
            addressCity = "";
            addressBarangay = "";
            addressSubdivision = "";
            addressHouseLotBlockNo = "";
            addressRmFlrUnitNoBldgName = "";
            gender = "";
            dateOfBirth = "";
            leftPrimaryFingerCode = "";
            rightPrimaryFingerCode = "";
            leftBackupFingerCode = "";
            rightBackupFingerCode = "";
            leftPrimaryFingerprint = "";
            rightPrimaryFingerprint = "";
            leftBackupFingerprint = "";
            rightBackupFingerprint = "";
            picture = "";
            height = "";
            weight = "";
            distinguishingFeatures = "";
            placeOfBirthCity = "";
            placeOfBirthProvince = "";
            placeOfBirthCountry = "";
            fatherFirstName = "";
            fatherMiddleName = "";
            fatherLastName = "";
            fatherSuffix = "";
            motherFirstName = "";
            motherMiddleName = "";
            motherLastName = "";
            motherSuffix = "";
            tin = "";
            cardStatus = "";
        }

        public short VerifyFingerprint(string fingerprint, ref string errMsg)
        {
            //byte[] ansi2 = System.IO.File.ReadAllBytes(@"D:\WORK\SSS-UBP RF\LT.ansi-fmr");
            //byte[] ansi2 = System.IO.File.ReadAllBytes(@"D:\WORK\SSS-UBP RF\EDEL 0511\EDEL 0511_RT.ansi-fmr");

            byte[] ansi2 = System.Convert.FromBase64String(leftPrimaryFingerprint);            
            bool matchResponse = sssUmidSrvcs.Helpers.ANSI.MatchTemplates(fingerprint, ansi2, ref errMsg);
            if (!matchResponse)
            {
                ansi2 = System.Convert.FromBase64String(rightPrimaryFingerprint);
                matchResponse = sssUmidSrvcs.Helpers.ANSI.MatchTemplates(fingerprint, ansi2, ref errMsg);
            }
            if (!matchResponse)
            {
                ansi2 = System.Convert.FromBase64String(leftBackupFingerprint);
                matchResponse = sssUmidSrvcs.Helpers.ANSI.MatchTemplates(fingerprint, ansi2, ref errMsg);
            }
            if (!matchResponse)
            {
                ansi2 = System.Convert.FromBase64String(rightBackupFingerprint);
                matchResponse = sssUmidSrvcs.Helpers.ANSI.MatchTemplates(fingerprint, ansi2, ref errMsg);
            }

            //if (!matchResponse)
            //{
            //    ansi2 = System.IO.File.ReadAllBytes(@"D:\WORK\SSS-UBP RF\LT.ansi-fmr");
            //    matchResponse = sssUmidSrvcs.Helpers.ANSI.MatchTemplates(fingerprint, ansi2, ref errMsg);
            //}

            if (matchResponse) return 0;
            else
            {
                if (errMsg == "") return 1;
                else return 2;
            }
        }
    }
}
