namespace de.fearvel.net.DataTypes
{
    /// <summary>
    /// A way to get a hidden key and a visible value for use in a Combobox
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ComboBoxKeyValueWrap
    {
        /// <summary>
        /// Key int
        /// </summary>
        public int Key { get; private set; }

        /// <summary>
        /// ValueString
        /// this will be shown in the ComboBox
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Constructor
        /// sets the Key and Value Properties
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public ComboBoxKeyValueWrap(int key, string val)
        {
            Key = key;
            Value = val;
        }

       /// <summary>
       /// This ToString function is the reason why
       /// something else than the ClassName will be displayed in the ComboBox 
       /// </summary>
       /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }
    }
}