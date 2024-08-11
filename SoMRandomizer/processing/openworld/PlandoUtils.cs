using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System.Collections.Generic;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// Utilities for processing Plando settings.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class PlandoUtils
    {
        // extract (and un-obfuscate if needed) plando setting string into a map on the context
        public static void processPlandoSetting(RandoSettings settings, RandoContext context)
        {
            // process plandomizer settings
            if (settings.get(OpenWorldSettings.PROPERTYNAME_PLANDO) != "")
            {
                string plandoValue = settings.get(OpenWorldSettings.PROPERTYNAME_PLANDO);
                if (plandoValue.StartsWith("#"))
                {
                    plandoValue = DataUtil.Base64Decode(plandoValue.Substring(1));
                }
                Logging.log("Plando settings without obfuscate: " + plandoValue, "spoiler");
                string[] individualSettings = plandoValue.Split(new char[] { ';' });
                foreach (string individualSetting in individualSettings)
                {
                    if (individualSetting.Length > 0)
                    {
                        string[] keyValue = individualSetting.Split(new char[] { ':' });
                        if (keyValue.Length == 2)
                        {
                            string kvKey = keyValue[0];
                            string kvValue = keyValue[1];
                            if (kvKey.StartsWith("p_") || kvKey.StartsWith("P_"))
                            {
                                List<string> locPrizes;
                                if (context.plandoSettings.ContainsKey(kvKey))
                                {
                                    locPrizes = context.plandoSettings[kvKey];
                                }
                                else
                                {
                                    locPrizes = new List<string>();
                                    context.plandoSettings[kvKey] = locPrizes;
                                }
                                locPrizes.Add(kvValue);
                            }
                        }
                    }
                }
            }
        }
    }
}
