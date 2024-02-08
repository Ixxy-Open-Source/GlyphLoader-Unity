// <copyright file="NameEncoding.cs" company="WaterTrans">
// © 2020 WaterTrans and Contributors
// </copyright>

using System;
using System.Text;

namespace WaterTrans.GlyphLoader.Internal.NAME
{
    /// <summary>
    /// NAME NameRecord name string encoding.
    /// </summary>
    internal class NameEncoding
    {
        /// <summary>
        /// Returns an encoding for the specified NameRecord.
        /// </summary>
        /// <param name="record">The <see cref="NameRecord"/>.</param>
        /// <returns>The encoding that is associated with the specified code page.</returns>
        internal static Encoding GetEncoding(NameRecord record)
        {
            try
            {
                switch (record.PlatformID)
                {
                    case (ushort)PlatformID.Unicode:
                        return Encoding.BigEndianUnicode;
                    case (ushort)PlatformID.Macintosh:
                        switch (record.EncodingID)
                        {
                            case 0: return Encoding.ASCII;
                        }

                        break;
                    case (ushort)PlatformID.ISO:
                        switch (record.EncodingID)
                        {
                            case 0: return Encoding.ASCII;
                            case 1: return Encoding.BigEndianUnicode;
                            case 2: return Encoding.GetEncoding(1252);
                        }

                        break;
                    case (ushort)PlatformID.Microsoft:
                        switch (record.EncodingID)
                        {
                            case 0: return Encoding.BigEndianUnicode;
                            case 1: return Encoding.BigEndianUnicode;
                            case 2: return Encoding.GetEncoding("shift_jis");
                            case 3: return Encoding.GetEncoding("gb2312");
                            case 4: return Encoding.GetEncoding("big5");
                            case 5: return Encoding.GetEncoding("x-cp20949");
                            case 6: return Encoding.GetEncoding("Johab");
                            case 10: return Encoding.BigEndianUnicode;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Encoding.ASCII;
            }

            return Encoding.ASCII;
        }
    }
}
