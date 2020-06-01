using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMIDLibrary;

namespace sssUmidSrvcs.Helpers
{
    public class UMIDLibrary
    {

        private static AllCardTech_Smart_Card sc;
        private static AllCardTech_Util util = new AllCardTech_Util();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static bool InitSC()
        {
            if (sc == null)
            {
                sc = new AllCardTech_Smart_Card();
                sc.InitializeReaders();
                return true;
            }
            else
            {               
                return true;
            }
        }

        public static bool SelectApplet(int contactless, int sam)
        {
            try
            {
                InitSC();
                if (sc.SelectApplet(contactless, sam)) return true;
                else return false;
            }
            catch (Exception ex)
            {                
                return false;
            }
        }

        public static short GetReaders(ref List<string> readers, ref string errMsg)
        {
            try
            {
                sc = new AllCardTech_Smart_Card();
                sc.InitializeReaders();
                foreach (String s in sc.ReaderList)
                {
                    if (s != null) readers.Add(s);
                }
                sc.Dispose();
                sc = null;

                if (readers.Count == 0) return 1;
                else return 0;                    
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                logger.Error(errMsg);
                return 2;
            }
        }

        public static short ConnectCard(int contactless, int sam, ref string errMsg)
        {         
            try
            {                
                if (SelectApplet(contactless, sam)) return 0;
                else
                {
                    logger.Error("Failed to select applet");
                    return 1; //failed to select applet
                }
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
                logger.Error(ex.Message);
                return 2; //error or no umid card
            };            
        }

        public static short AuthenticateCard(int contactless, int sam, ref string errMsg)
        {
            string CRN = "";
            try
            {
                if (SelectApplet(contactless, sam)) CRN = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.CRN));
                else
                {
                    logger.Error("Failed to select applet");
                    return 1; //failed to select applet
                } 

                if (CRN != "") return 0;
                else
                {
                    logger.Error("No crn extracted");
                    return 1; 
                }
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
                logger.Error(ex.Message);
                return 2; //error or no umid card
            };
        }

        public static short ReadData(Models.requestParam requestParam, Models.CardData cardData, ref string errMsg)
        {            
            try
            {
                sc = new AllCardTech_Smart_Card();
                sc.InitializeReaders();
                if (SelectApplet(requestParam.contactless, requestParam.sam))
                {
                    AllCardTech_Util util = new AllCardTech_Util();
                    cardData.CRN = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.CRN));
                                    
                    //string crnFolder = string.Format(@"{0}\{1}", System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "App_Data"), cardData.CRN);
                    string crnFolder = string.Format(@"{0}\{1}", @"C:\Allcard\SSS UMID", cardData.CRN);
                    if (!System.IO.Directory.Exists(crnFolder)) System.IO.Directory.CreateDirectory(crnFolder);

                    string cardStatus = "";
                    if (sc.GetCardStatus(ref cardStatus)) cardData.cardStatus = cardStatus;

                    if (sc.AuthenticateSL1())
                    {
                        //byte[] bytePIN = System.Text.Encoding.ASCII.GetBytes("123456");
                        //if (sc.AuthenticateSL2(bytePIN))
                        //{
                        string Photo_File = string.Format("{0}\\Photo.jpg", crnFolder);
                        string LP_File = string.Format("{0}\\Lprimary.ansi-fmr", crnFolder);
                        string LB_File = string.Format("{0}\\Lbackup.ansi-fmr", crnFolder);
                        string RP_File = string.Format("{0}\\Rprimary.ansi-fmr", crnFolder);
                        string RB_File = string.Format("{0}\\Rbackup.ansi-fmr", crnFolder);

                        sc.getUmidFile(Photo_File, AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_PICTURE);
                        sc.getUmidFile(LP_File, AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_LEFT_PRIMARY_FINGER);
                        sc.getUmidFile(RP_File, AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_RIGHT_PRIMARY_FINGER);
                        sc.getUmidFile(LB_File, AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_LEFT_SECONDARY_FINGER);
                        sc.getUmidFile(RB_File, AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_RIGHT_SECONDARY_FINGER);

                        cardData.firstName = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.FIRST_NAME));
                        cardData.middleName = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.MIDDLE_NAME));
                        cardData.lastName = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.LAST_NAME));
                        cardData.suffix = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.SUFFIX));
                        cardData.addressPostalCode = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.ADDRESS_POSTAL_CODE));
                        cardData.addressCountry = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.ADDRESS_COUNTRY));
                        cardData.addressProvince = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.ADDRESS_PROVINCIAL_OR_STATE));
                        cardData.addressCity = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.ADDRESS_CITY_OR_MUNICIPALITY));
                        cardData.addressBarangay = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.ADDRESS_BARANGAY_OR_DISTRIC_OR_LOCALITY));
                        cardData.addressSubdivision = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.ADDRESS_SUBDIVISION));
                        cardData.addressHouseLotBlockNo = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.ADDRESS_HOUSE_OR_LOT_AND_BLOCK_NUMBER));
                        cardData.addressRmFlrUnitNoBldgName = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.ADDRESS_ROOM_OR_FLOOR_OR_UNIT_NO_AND_BUILDING_NAME));
                        cardData.gender = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.GENDER));

                        string dob = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.DATE_OF_BIRTH));
                        if (dob.Length > 7) cardData.dateOfBirth = string.Format("{0}/{1}/{2}", dob.Substring(4,2), dob.Substring(6, 2), dob.Substring(0,4));
                        else cardData.dateOfBirth = dob;
                        
                        cardData.leftPrimaryFingerCode = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.LEFT_PRIMARY_FINGER_CODE));
                        cardData.rightPrimaryFingerCode = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.RIGHT_PRIMARY_FINGER_CODE));
                        cardData.leftBackupFingerCode = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.LEFT_SECONDARY_FINGER_CODE));
                        cardData.rightBackupFingerCode = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.RIGHT_SECONDARY_FINGER_CODE));
                        if (System.IO.File.Exists(Photo_File)) cardData.picture = Convert.ToBase64String(System.IO.File.ReadAllBytes(Photo_File));
                        if (System.IO.File.Exists(LP_File)) cardData.leftPrimaryFingerprint = Convert.ToBase64String(System.IO.File.ReadAllBytes(LP_File));
                        if (System.IO.File.Exists(RP_File)) cardData.rightPrimaryFingerprint = Convert.ToBase64String(System.IO.File.ReadAllBytes(RP_File));
                        if (System.IO.File.Exists(LB_File)) cardData.leftBackupFingerprint = Convert.ToBase64String(System.IO.File.ReadAllBytes(LB_File));
                        if (System.IO.File.Exists(RB_File)) cardData.rightBackupFingerprint = Convert.ToBase64String(System.IO.File.ReadAllBytes(RB_File));

                        cardData.height = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.HEIGHT));
                        cardData.weight = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.WEIGHT));
                        cardData.distinguishingFeatures = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.DISTINGUISHING_FEATURES));
                        cardData.placeOfBirthCity = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.PLACE_OF_BIRTH_CITY));
                        cardData.placeOfBirthProvince = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.PLACE_OF_BIRTH_PROVINCE));
                        cardData.placeOfBirthCountry = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.PLACE_OF_BIRTH_COUNTRY));
                        cardData.fatherFirstName = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.FATHER_FIRST_NAME));
                        cardData.fatherMiddleName = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.FATHER_MIDDLE_NAME));
                        cardData.fatherLastName = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.FATHER_LAST_NAME));
                        cardData.fatherSuffix = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.FATHER_SUFFIX));
                        cardData.motherFirstName = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.MOTHER_FIRST_NAME));
                        cardData.motherMiddleName = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.MOTHER_MIDDLE_NAME));
                        cardData.motherLastName = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.MOTHER_LAST_NAME));
                        cardData.motherSuffix = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.MOTHER_SUFFIX));
                        cardData.tin = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.TIN));
                        return 0;
                    }
                    else
                    {
                        logger.Error(cardData.CRN + " SL1 failed");
                        return 2;
                    }
                }
                else
                {
                    logger.Error("Failed to select applet");
                    return 1; //failed to select applet
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                logger.Error(ex.Message);
                return 3;
            }
        }

        public static short ReadBio(Models.requestParam requestParam, Models.CardData cardData, ref string errMsg)
        {
            try
            {                
                if (SelectApplet(requestParam.contactless, requestParam.sam))
                {                   
                    AllCardTech_Util util = new AllCardTech_Util();
                    cardData.CRN = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.CRN));

                    //string crnFolder = string.Format(@"{0}\{1}", System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "App_Data"), cardData.CRN);
                    string crnFolder = string.Format(@"{0}\{1}", @"C:\Allcard\SSS UMID", cardData.CRN);
                    if (!System.IO.Directory.Exists(crnFolder)) System.IO.Directory.CreateDirectory(crnFolder);

                    string cardStatus = "";
                    if (sc.GetCardStatus(ref cardStatus)) cardData.cardStatus = cardStatus;

                    if (sc.AuthenticateSL1())
                    {
                        string LP_File = string.Format("{0}\\Lprimary.ansi-fmr", crnFolder);
                        string LB_File = string.Format("{0}\\Lbackup.ansi-fmr", crnFolder);
                        string RP_File = string.Format("{0}\\Rprimary.ansi-fmr", crnFolder);
                        string RB_File = string.Format("{0}\\Rbackup.ansi-fmr", crnFolder);

                        sc.getUmidFile(LP_File, AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_LEFT_PRIMARY_FINGER);
                        sc.getUmidFile(RP_File, AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_RIGHT_PRIMARY_FINGER);
                        sc.getUmidFile(LB_File, AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_LEFT_SECONDARY_FINGER);
                        sc.getUmidFile(RB_File, AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_RIGHT_SECONDARY_FINGER);

                        if (System.IO.File.Exists(LP_File)) cardData.leftPrimaryFingerprint = Convert.ToBase64String(System.IO.File.ReadAllBytes(LP_File));
                        if (System.IO.File.Exists(RP_File)) cardData.rightPrimaryFingerprint = Convert.ToBase64String(System.IO.File.ReadAllBytes(RP_File));
                        if (System.IO.File.Exists(LB_File)) cardData.leftBackupFingerprint = Convert.ToBase64String(System.IO.File.ReadAllBytes(LB_File));
                        if (System.IO.File.Exists(RB_File)) cardData.rightBackupFingerprint = Convert.ToBase64String(System.IO.File.ReadAllBytes(RB_File));

                        return 0;
                    }
                    else
                    {
                        logger.Error(cardData.CRN + " SL1 failed");
                        return 2;
                    }
                }
                else
                {
                    logger.Error("Failed to select applet");
                    return 1; //failed to select applet
                }                
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                logger.Error(ex.Message);
                return 3;
            }
        }

    }
}
