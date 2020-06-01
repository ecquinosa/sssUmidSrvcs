using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sssUmidSrvcs.Helpers
{
    public class Utilities
    {

        public static string responseMessage(string code)
        {
            //someBool ? "true" : "false";

            switch (code)
            {                
                case "401A":
                    return "Authentication error";
                case "401B":
                    return "Fingerprint did not match";
                case "412A":
                    return "No card readers found";
                case "412B":
                    return "Failed to connect to card";
                case "500A":
                    return "Invalid request";                
            }

            return "OK";
        }

    }
}
