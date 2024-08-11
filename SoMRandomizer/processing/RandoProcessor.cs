using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System;

namespace SoMRandomizer.processing
{
    /// <summary>
    /// Superclass for a toggle-able hack or data processor for other hacks
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public abstract class RandoProcessor
    {
        public void add(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context)
        {
            int prevOffset = context.workingOffset;
            try
            {
                bool success = process(origRom, outRom, seed, settings, context);
                if (success)
                {
                    int offsetAfter = context.workingOffset;
                    int hackSize = offsetAfter - prevOffset;
                    if (hackSize > 0)
                    {
                        Logging.log("Adding: [" + getName() + "] at 0x" + prevOffset.ToString("X6") + " -> 0x" + (offsetAfter - 1).ToString("X6") + "(0x" + hackSize.ToString("X") + ") bytes");
                    }
                    else
                    {
                        Logging.log("Adding: [" + getName() + "]");
                    }
                }
                else
                {
                    Logging.log("Skipping: [" + getName() + "]");
                }
            }
            catch (Exception e)
            {
                Logging.log("EXCEPTION in: [" + getName() + "]: " + e.Message + "\nStack trace:\n" + e.StackTrace);
                throw e; // caught by higher-level stuff to fail the rom generation
            }
        }
        protected abstract bool process(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context);
        protected abstract string getName();
    }
}
