using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SoMRandomizer.config.settings
{
    /// <summary>
    /// a collection of string properties, each with a default/initial value, that can be reinterpreted as a couple other types
    /// based on how we set it from the UI
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class StringValueSettings
    {
        // ensure decimal values go out with a period, not a comma
        private static CultureInfo enUs = new CultureInfo("en-US");
        private const string BOOLEAN_TRUE_VALUE = "yes";
        private const string BOOLEAN_FALSE_VALUE = "no";
        // acceptable values for "yes" when converting strings to booleans. case insensitive
        private static string[] BOOLEAN_TRUE_VALUES = new string[] { "yes", "true", "1" };
        private const string BOOLEAN_TRUE_DISPLAY_VALUE = "Yes";
        private const string BOOLEAN_FALSE_DISPLAY_VALUE = "No";
        private Dictionary<string, string> defaults = new Dictionary<string, string>();
        private Dictionary<string, string> currents = new Dictionary<string, string>();
        private Dictionary<string, StringEnumEntrySettings> stringEnumEntrySettings = new Dictionary<string, StringEnumEntrySettings>();
        private Dictionary<string, DecimalEntrySettings> decimalValueEntrySettings = new Dictionary<string, DecimalEntrySettings>();

        public bool hasSetting(string key)
        {
            return currents.ContainsKey(key);
        }

        public List<string> keysStartingWith(string prefix)
        {
            List<string> keys = new List<string>();
            foreach (string key in currents.Keys)
            {
                if(key.StartsWith(prefix))
                {
                    keys.Add(key);
                }
            }
            return keys;
        }

        // for non-enumerated string values (plando)
        protected void setInitial(string name, string value)
        {
            defaults[name] = value;
            currents[name] = value;
        }

        // -----------------------
        // bool values
        // -----------------------
        protected void setInitial(string name, bool value)
        {
            defaults[name] = boolToString(value);
            currents[name] = boolToString(value);
            // treat as string enum w/ yes/no
            stringEnumEntrySettings[name] = new StringEnumEntrySettings();
            stringEnumEntrySettings[name].displayValues = new string[] { BOOLEAN_TRUE_DISPLAY_VALUE, BOOLEAN_FALSE_DISPLAY_VALUE };
            stringEnumEntrySettings[name].optionValues = new string[] { BOOLEAN_TRUE_VALUE, BOOLEAN_FALSE_VALUE };
        }

        public virtual bool getBool(string key)
        {
            return stringToBool(get(key));
        }

        public void setBool(string key, bool val)
        {
            currents[key] = boolToString(val);
        }

        public bool getDefaultBool(string key)
        {
            return stringToBool(getDefault(key));
        }

        public static bool stringToBool(string str)
        {
            //return str == BOOLEAN_TRUE_VALUE;
            return BOOLEAN_TRUE_VALUES.Contains(str.ToLower().Trim());
        }

        public static string boolToString(bool b)
        {
            return b ? BOOLEAN_TRUE_VALUE : BOOLEAN_FALSE_VALUE;
        }

        // -----------------------
        // double values
        // -----------------------
        protected void setInitial(string name, double value, double min, double max, int decimalPlaces)
        {
            // for numeric up/downs; associated double value w/ min/max
            defaults[name] = doubleToString(value);
            currents[name] = doubleToString(value);
            decimalValueEntrySettings[name] = new DecimalEntrySettings();
            decimalValueEntrySettings[name].min = min;
            decimalValueEntrySettings[name].max = max;
            decimalValueEntrySettings[name].decimalPlaces = decimalPlaces;
        }

        public virtual double getDouble(string key)
        {
            try
            {
                return Double.Parse(get(key), enUs);
            }
            catch(Exception e)
            {
                return Double.NaN;
            }
        }

        public void setDouble(string key, double val)
        {
            set(key, doubleToString(val));
        }

        public double getDoubleDefault(string key)
        {
            return stringToDouble(getDefault(key));
        }

        public double getDoubleMinimum(string name)
        {
            return decimalValueEntrySettings[name].min;
        }

        public double getDoubleMaximum(string name)
        {
            return decimalValueEntrySettings[name].max;
        }

        public int getDecimalPlaces(string name)
        {
            return decimalValueEntrySettings[name].decimalPlaces;
        }

        public static double stringToDouble(string str)
        {
            return Double.Parse(str, enUs);
        }

        public static string doubleToString(double d)
        {
            return d.ToString(enUs);
        }

        // -----------------------
        // integers
        // -----------------------
        protected void setInitial(string name, int value)
        {
            // for numeric up/downs; associated double value w/ min/max
            defaults[name] = "" + value;
            currents[name] = "" + value;
        }

        public virtual int getInt(string key)
        {
            return Int32.Parse(get(key), enUs);
        }

        public virtual int getIntAndIncrement(string key)
        {
            int val = Int32.Parse(get(key), enUs);
            setInt(key, val + 1);
            return val;
        }

        public void setInt(string key, int val)
        {
            set(key, "" + val);
        }

        public int getIntDefault(string key)
        {
            return Int32.Parse(key);
        }

        // -----------------------
        // string enumerations
        // -----------------------
        protected void setInitial(string name, string[] enumOptionsValues, string[] enumDisplayValues, string value)
        {
            // note that "value" is the initial NON-DISPLAY value.
            // for combo boxes and trackbars - string enumerations
            defaults[name] = value;
            currents[name] = value;
            stringEnumEntrySettings[name] = new StringEnumEntrySettings();
            stringEnumEntrySettings[name].optionValues = enumOptionsValues;
            stringEnumEntrySettings[name].displayValues = enumDisplayValues;
        }

        public virtual string get(string key)
        {
            return currents.ContainsKey(key) ? currents[key] : "";
        }

        public string getDefault(string key)
        {
            return defaults[key];
        }

        public int getDefaultIndex(string key)
        {
            return stringEnumEntrySettings[key].optionValues.ToList().IndexOf(defaults[key]);
        }

        public int getIndex(string key)
        {
            return stringEnumEntrySettings[key].optionValues.ToList().IndexOf(currents[key]);
        }

        public int getIndex(string key, string value)
        {
            return stringEnumEntrySettings[key].optionValues.ToList().IndexOf(value);
        }

        public string[] getDisplayValues(string key)
        {
            return stringEnumEntrySettings[key].displayValues;
        }

        public string[] getOptionsValues(string key)
        {
            return stringEnumEntrySettings[key].optionValues;
        }

        public virtual void set(string key, string val)
        {
            currents[key] = val;
        }

        public virtual void set(string key, int index)
        {
            currents[key] = stringEnumEntrySettings[key].optionValues[index];
        }


        // -----------------------
        // string[] enumerations
        // -----------------------
        protected void setInitial(string name, string[] enumOptionsValues, string[] enumDisplayValues, string[] values)
        {
            // for checked listbox control that i use for ancient cave biomes selection
            defaults[name] = arrayToSingleString(values);
            currents[name] = arrayToSingleString(values);
            stringEnumEntrySettings[name] = new StringEnumEntrySettings();
            stringEnumEntrySettings[name].optionValues = enumOptionsValues;
            stringEnumEntrySettings[name].displayValues = enumDisplayValues;
        }

        public string[] getArray(string key)
        {
            return currents.ContainsKey(key) ? singleStringToArray(currents[key]) : new string[] { };
        }

        public string[] getDefaultArray(string key)
        {
            return currents.ContainsKey(key) ? singleStringToArray(defaults[key]) : new string[] { };
        }

        public int[] getDefaultIndexes(string key)
        {
            List<int> indexes = new List<int>();
            string[] strs = getDefaultArray(key);
            strs.ToList().ForEach(str => indexes.Add(stringEnumEntrySettings[key].optionValues.ToList().IndexOf(str)));
            return indexes.ToArray();
        }

        public int[] getIndexes(string key)
        {
            List<int> indexes = new List<int>();
            string[] strs = getArray(key);
            strs.ToList().ForEach(str => indexes.Add(stringEnumEntrySettings[key].optionValues.ToList().IndexOf(str)));
            return indexes.ToArray();
        }

        public virtual void set(string key, string[] vals)
        {
            currents[key] = arrayToSingleString(vals);
        }

        public void setIntArray(string key, int[] vals)
        {
            currents[key] = arrayToSingleString(vals);
        }

        public int[] getIntArray(string key)
        {
            return singleStringToIntArray(get(key));
        }

        public void setIndexes(string key, int[] indexes)
        {
            string[] vals = new string[indexes.Length];
            for (int i=0; i < indexes.Length; i++)
            {
                vals[i] = stringEnumEntrySettings[key].optionValues[indexes[i]];
            }
            currents[key] = arrayToSingleString(vals);
        }

        private string arrayToSingleString(string[] array)
        {
            return String.Join(",", array);
        }

        private string arrayToSingleString(int[] array)
        {
            return String.Join(",", array);
        }

        private string[] singleStringToArray(string str)
        {
            return str.Split(new char[] { ',' });
        }

        private int[] singleStringToIntArray(string str)
        {
            string[] strArray = singleStringToArray(str);
            int[] intArray = new int[strArray.Length];
            for(int i=0; i < strArray.Length; i++)
            {
                intArray[i] = Int32.Parse(strArray[i]);
            }
            return intArray;
        }

        public override string ToString()
        {
            string desc = "Properties [" + GetType().Name + "]:";
            foreach(string key in currents.Keys)
            {
                desc += Environment.NewLine + "  " + key + " = " + currents[key];
            }
            return desc;
        }

        private class DecimalEntrySettings
        {
            public double max;
            public double min;
            public int decimalPlaces;
        }

        private class StringEnumEntrySettings
        {
            public string[] optionValues;
            public string[] displayValues;
        }
    }
}
