using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Business_Layer.services;
using sssUmidSrvcs.Helpers;
using sssUmidSrvcs.Models;

namespace sssUmidSrvcs.Controllers
{
    public class ValuesController : ApiController
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        ComputerInfo computerInfo = new ComputerInfo();

        //[Route("~/test")]
        //[HttpGet]
        //public IHttpActionResult test()
        //{
        //Cryptor cryptor = new Cryptor();
        //string CryptedText = cryptor.Encrypt(ComputerInfo.ProcIDMachName);
        //computerInfo.WriteKeys(CryptedText);
        //string ReadKeys = ComputerInfo.ReadKeys;

        //ComputerInfo computerInfo = new ComputerInfo();

        //if (computerInfo.VerifyKeys() == false)
        //{
        //    return Content(HttpStatusCode.InternalServerError, new response { message = "Authentication Failed." });
        //}

        //return Ok(new { ReadKeys = ReadKeys });
        //}


        [Route("~/api/GetReaders")]
        [HttpPost]
        public IHttpActionResult GetReaders()
        {
            //Authentication to AllcardTech
            if (computerInfo.VerifyKeys() == false)
            {
                return Content(HttpStatusCode.InternalServerError, new response { message = "Authentication Failed." });
            }

            List<string> readers = new List<string>();

            string errMsg = "";
            var response = sssUmidSrvcs.Helpers.UMIDLibrary.GetReaders(ref readers, ref errMsg);

            switch (response)
            {
                case 0:
                    return Ok(new Models.Device() { readers = readers.ToArray() });
                case 1:
                    return Content(HttpStatusCode.PreconditionFailed, new response { message = Helpers.Utilities.responseMessage("412A") });
                default:
                    return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
            }
        }


        [Route("~/api/ReadCard")]
        [HttpPost]
        public IHttpActionResult ReadCard([FromBody] requestParam requestParam)
        {
            //Authentication to AllcardTech
            if (computerInfo.VerifyKeys() == false)
            {
                return Content(HttpStatusCode.InternalServerError, new response { message = "Authentication Failed." });
            }

            CardData cardData = new CardData();
            string errMsg = "";

            short cnresponse = sssUmidSrvcs.Helpers.UMIDLibrary.ConnectCard(requestParam.contactless, requestParam.sam, ref errMsg);
            short aresponse = sssUmidSrvcs.Helpers.UMIDLibrary.AuthenticateCard(requestParam.contactless, requestParam.sam, ref errMsg);
            short rresponse = sssUmidSrvcs.Helpers.UMIDLibrary.ReadData(requestParam, cardData, ref errMsg);

            if (cnresponse != 0)
            {
                switch (cnresponse)
                {
                    case 1:
                        return Content(HttpStatusCode.PreconditionFailed, new response { message = Helpers.Utilities.responseMessage("412B") });
                    default:
                        return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
                }
            }
            else if (aresponse != 0)
            {
                switch (aresponse)
                {
                    case 1:
                        return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401A") });
                    default:
                        return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
                }
            }
            else
            {
                switch (rresponse)
                {
                    case 0:
                        return Ok(cardData);
                    case 1:
                    case 2:
                        return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401A") });
                    default:
                        return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
                }
            }
        }

        [Route("~/api/ReadCardWithAuthentication")]
        [HttpPost]
        public IHttpActionResult ReadCardWithAuthentication([FromBody] requestParam requestParam)
        {
            //Authentication to AllcardTech
            if (computerInfo.VerifyKeys() == false)
            {
                return Content(HttpStatusCode.InternalServerError, new response { message = "Authentication Failed." });
            }

            CardData cardData = new CardData();
            string errMsg = "";

            short cnresponse = sssUmidSrvcs.Helpers.UMIDLibrary.ConnectCard(requestParam.contactless, requestParam.sam, ref errMsg);
            short aresponse = sssUmidSrvcs.Helpers.UMIDLibrary.AuthenticateCard(requestParam.contactless, requestParam.sam, ref errMsg);
            short rresponse = sssUmidSrvcs.Helpers.UMIDLibrary.ReadData(requestParam, cardData, ref errMsg);
            if (cnresponse != 0)
            {
                switch (cnresponse)
                {
                    case 1:
                        return Content(HttpStatusCode.PreconditionFailed, new response { message = Helpers.Utilities.responseMessage("412B") });
                    default:
                        return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
                }
            }
            else if (aresponse != 0)
            {
                switch (aresponse)
                {
                    case 1:
                        return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401A") });
                    default:
                        return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
                }
            }
            else
            {
                switch (rresponse)
                {
                    case 0:
                        rresponse = cardData.VerifyFingerprint(requestParam.fingerprint, ref errMsg);

                        switch (rresponse)
                        {
                            case 0:
                                return Ok(cardData);
                            default:
                                if (errMsg == "") return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401B") });
                                else
                                {
                                    logger.Error(cardData.CRN + " " + errMsg);
                                    return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
                                }
                        }
                    case 1:
                    case 2:
                        return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401A") });
                    default:
                        return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
                }
            }
        }


        //[Route("~/api/VerifyFingerprint")]
        //[HttpPost]
        //public IHttpActionResult VerifyFingerprint([FromBody] requestParam requestParam)
        //{
        //    //byte[] ansi2 = System.IO.File.ReadAllBytes(@"D:\WORK\SSS-UBP RF\LT.ansi-fmr");
        //    //byte[] ansi2 = System.IO.File.ReadAllBytes(@"D:\WORK\SSS-UBP RF\EDEL 0511\EDEL 0511_RT.ansi-fmr");

        //    CardData cardData = new CardData();
        //    string errMsg = "";
        //    short response = sssUmidSrvcs.Helpers.UMIDLibrary.ReadBio(requestParam, cardData, ref errMsg);

        //    switch (response)
        //    {
        //        case 0:
        //            //1 to 1 matching                    
        //            response = cardData.VerifyFingerprint(requestParam.fingerprint, ref errMsg);

        //            switch (response)
        //            {
        //                case 0:
        //                    return Ok("OK");
        //                default:
        //                    if (errMsg == "") return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401B") });
        //                    else
        //                    {
        //                        logger.Error(cardData.CRN + " " + errMsg);
        //                        return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
        //                    }
        //            }
        //        case 1:
        //        case 2:
        //            return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401A") });
        //        default:
        //            return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
        //    }
        //}
        //[Route("~/api/ConnectCard/{contactless}/{sam}")]
        //[HttpPost]
        //public IHttpActionResult ConnectCard(int contactless, int sam)
        //{
        //    string errMsg = "";
        //    short response = sssUmidSrvcs.Helpers.UMIDLibrary.ConnectCard(contactless, sam, ref errMsg);

        //    switch (response)
        //    {
        //        case 0:
        //            return Ok("OK");
        //        case 1:
        //            return Content(HttpStatusCode.PreconditionFailed, new response { message = Helpers.Utilities.responseMessage("412B") });
        //        default:
        //            return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
        //    }
        //}

        //[Route("~/api/AuthenticateCard/{contactless}/{sam}")]
        //[HttpPost]
        //public IHttpActionResult AuthenticateCard(int contactless, int sam)
        //{
        //    string errMsg = "";
        //    short response = sssUmidSrvcs.Helpers.UMIDLibrary.AuthenticateCard(contactless, sam, ref errMsg);

        //    switch (response)
        //    {
        //        case 0:
        //            return Ok("OK");
        //        case 1:
        //            return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401A") });
        //        default:
        //            return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
        //    }
        //}

        //[Route("~/api/ReadCard")]
        //[HttpPost]
        //public IHttpActionResult ReadCard([FromBody] requestParam requestParam)
        //{
        //    CardData cardData = new CardData();
        //    string errMsg = "";
        //    short response = sssUmidSrvcs.Helpers.UMIDLibrary.ReadData(requestParam, cardData, ref errMsg);

        //    switch (response)
        //    {
        //        case 0:
        //            return Ok(cardData);
        //        case 1:
        //        case 2:
        //            return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401A") });
        //        default:
        //            return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
        //    }
        //}
        //[Route("~/api/ReadCardWithAuthentication")]
        //[HttpPost]
        //public IHttpActionResult ReadCardWithAuthentication([FromBody] requestParam requestParam)
        //{
        //    CardData cardData = new CardData();
        //    string errMsg = "";
        //    short response = sssUmidSrvcs.Helpers.UMIDLibrary.ReadData(requestParam, cardData, ref errMsg);

        //    switch (response)
        //    {
        //        case 0:
        //            response = cardData.VerifyFingerprint(requestParam.fingerprint, ref errMsg);

        //            switch (response)
        //            {
        //                case 0:
        //                    return Ok(cardData);
        //                default:
        //                    if (errMsg == "") return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401B") });
        //                    else
        //                    {
        //                        logger.Error(cardData.CRN + " " + errMsg);
        //                        return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
        //                    }
        //            }
        //        case 1:
        //        case 2:
        //            return Content(HttpStatusCode.Unauthorized, new response { message = Helpers.Utilities.responseMessage("401A") });
        //        default:
        //            return Content(HttpStatusCode.InternalServerError, new response { message = errMsg });
        //    }
        //}
    }

}
