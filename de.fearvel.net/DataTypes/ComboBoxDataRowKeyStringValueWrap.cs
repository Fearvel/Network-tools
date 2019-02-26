using System.Data;

namespace de.fearvel.net.DataTypes
{
    /// <summary>
    /// A way to get a hidden key and a visible value for use in a Combobox
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ComboBoxDataRowKeyStringValueWrap
    {
        /// <summary>
        /// DataRow
        /// </summary>
        public DataRow Row { get; private set; }

        /// <summary>
        /// ValueString
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public ComboBoxDataRowKeyStringValueWrap(DataRow row, string val)
        {
            Row = row;
            Value = val;
        }
       /// <summary>
       /// ToString()
       /// used to only display the value in the ComboBox
       /// </summary>
       /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }
    }
}