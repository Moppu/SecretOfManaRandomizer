using System;
using System.Collections.Generic;

namespace SoMRandomizer.config.settings
{
    /// <summary>
    /// Parent class to options for each mode's settings holders, that can find a settings in either the mode-specific settings
    /// or a CommonSettings.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public abstract class RandoSettings : StringValueSettings
    {
        protected CommonSettings commonSettings;

        public RandoSettings(CommonSettings commonSettings)
        {
            this.commonSettings = commonSettings;
        }

        public override string ToString()
        {
            return commonSettings.ToString() + Environment.NewLine + base.ToString();
        }

        public override string get(string key)
        {
            if(commonSettings.hasSetting(key))
            {
                return commonSettings.get(key);
            }
            return base.get(key);
        }

        public override bool getBool(string key)
        {
            if (commonSettings.hasSetting(key))
            {
                return commonSettings.getBool(key);
            }
            return base.getBool(key);
        }

        public override double getDouble(string key)
        {
            if (commonSettings.hasSetting(key))
            {
                return commonSettings.getDouble(key);
            }
            return base.getDouble(key);
        }

        public override int getInt(string key)
        {
            if (commonSettings.hasSetting(key))
            {
                return commonSettings.getInt(key);
            }
            return base.getInt(key);
        }

        public override void set(string key, int index)
        {
            if (commonSettings.hasSetting(key))
            {
                commonSettings.set(key, index);
            }
            else
            {
                base.set(key, index);
            }
        }

        public override void set(string key, string val)
        {
            if (commonSettings.hasSetting(key))
            {
                commonSettings.set(key, val);
            }
            else
            {
                base.set(key, val);
            }
        }

        public override void set(string key, string[] vals)
        {
            if (commonSettings.hasSetting(key))
            {
                commonSettings.set(key, vals);
            }
            else
            {
                base.set(key, vals);
            }
        }

        public void processNewSettings(Dictionary<string, string> settings)
        {
            foreach(string key in settings.Keys)
            {
                set(key, settings[key]);
            }
        }
    }
}
