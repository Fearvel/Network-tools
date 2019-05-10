using System.Data;

namespace de.fearvel.net.DataTypes
{
    /// <summary>
    /// A way to get a hidden key and a visible value for use in a Combobox
    /// This is nearly the same as the ComboBoxKeyValueWrap
    /// the difference is that this maps a DataRow to a in the ComboBox visible string 
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
        /// this will be shown in the ComboBox
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Constructor
        /// sets the Row and Value Properties
        /// </summary>
        /// <param name="row"></param>
        /// <param name="val"></param>
        public ComboBoxDataRowKeyStringValueWrap(DataRow row, string val)
        {
            Row = row;
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