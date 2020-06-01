
using System;
using SecuGen.FDxSDKPro.Windows;

namespace sssUmidSrvcs.Helpers
{
    public class ANSI
    {

        private static Int32 m_ImageWidth;
        private static SGFingerPrintManager m_FPM = new  SGFingerPrintManager();
        private static Int32 m_ImageHeight;

        public static bool MatchTemplates(string base64ansi1, byte[] ansiFile2, ref string ErrorMessage)
        {
            try
            {
                byte[] ansiFile1 = Convert.FromBase64String(base64ansi1);                

                Int32 iError;
                bool matched=false;

                iError = m_FPM.InitEx(m_ImageWidth, m_ImageHeight, 500);
                iError = m_FPM.MatchAnsiTemplate(ansiFile1, 0, ansiFile2, 0, SGFPMSecurityLevel.NORMAL, ref matched);               

                if (MatchedFingerprints(iError, matched))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        private static bool ConvertBase64ToFile(string id, string fileBase64, string fileName)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(fileBase64);
                System.IO.File.WriteAllBytes(fileName, imageBytes);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static bool MatchedFingerprints(int iError, bool matched)
        {
            if (iError == (int)SGFPMError.ERROR_NONE)
            {
                if (matched)
                    return true;
                else                    
                    return false;
            }
            else
              
                return false;
        }

    }
}
