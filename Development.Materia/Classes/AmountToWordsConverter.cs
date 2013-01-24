#region "imports"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics;
#endregion

namespace Development.Materia
{
    /// <summary>
    /// Numeric value to english word string converting class.
    /// </summary>
    public class AmountToWordsConverter
    {

        #region "constructors"
        /// <summary>
        /// Creates a new instance of AmountToWordsConverter.
        /// </summary>
        public AmountToWordsConverter() : this(0)   
        { }

        /// <summary>
        /// Creates a new instance of AmountToWordsConverter.
        /// </summary>
        /// <param name="amounttoconvert">Amount to be converted</param>
        public AmountToWordsConverter(double amounttoconvert) : this(amounttoconvert,"USD")
        { }

        /// <summary>
        /// Creates a new instance of AmountToWordsConverter.
        /// </summary>
        /// <param name="amounttoconvert">Amount to be converted</param>
        /// <param name="associatedcurrency">Associated currency</param>
        public AmountToWordsConverter(double amounttoconvert, string associatedcurrency)
        {  Amount = amounttoconvert; Currency = associatedcurrency; }

        #endregion

        #region  "properties"
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double _amount = 0;

        /// <summary>
        /// Gets or sets the amount to be converted into english-word.
        /// </summary>
        public double Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _currency = "USD";

        /// <summary>
        /// Gets or sets the currency associated with the english-word result.
        /// </summary>
        public string Currency
        {
            get { return _currency; }
            set { _currency = value; }
        }
        #endregion

        #region "methods"

        private string HundredsToWords(int numbertoconvert)
        {
            string _words = "";
            int _hundred =  numbertoconvert.WholePartDivision(100);
            
            _words = OnesToWords(_hundred);
            if (_words.Trim() != "") _words += " Hundred";

            return _words;
        }

        private string TensToWords(int numbertoconvert)
        {
            string _words = "";
            int _ten = numbertoconvert.WholePartDivision(10);

            switch (_ten)
            {
                case 1:
                    switch (numbertoconvert)
                    {
                        case 10:
                            _words = "Ten"; break;
                        case 11:
                            _words = "Eleven"; break;
                        case 12 :
                            _words = "Twelve"; break;
                        case 13:
                            _words = "Thirteen"; break;
                        case 14:
                            _words = "Fourteen"; break;
                        case 15:
                            _words = "Fifteen"; break;
                        case 16:
                            _words = "Sixteen"; break;
                        case 17:
                            _words = "Seventeen"; break;
                        case 18:
                            _words = "Eighteen"; break;
                        case 19:
                            _words = "Nineteen"; break;
                        default: break;
                    }
                    break;
                case 2:
                    _words = "Twenty"; break;
                case 3:
                    _words = "Thirty"; break;
                case 4:
                    _words = "Fourty"; break;
                case 5:
                    _words = "Fifty"; break;
                case 6:
                    _words = "Sixty"; break;
                case 7:
                    _words = "Seventy"; break;
                case 8:
                    _words = "Eighty"; break;
                case 9:
                    _words = "Ninety"; break;
                default: break;
            }

            return _words;
        }

        private string OnesToWords(int numbertoconvert)
        {
            string _words = "";

            switch (numbertoconvert)
            {
                case 1:
                    _words = "One"; break;
                case 2:
                    _words = "Two"; break;
                case 3:
                    _words = "Three"; break;
                case 4:
                    _words = "Four"; break;
                case 5:
                    _words = "Five"; break;
                case 6:
                    _words = "Six"; break;
                case 7:
                    _words = "Seven"; break;
                case 8:
                    _words = "Eight"; break;
                case 9:
                    _words = "Nine"; break;
                default: break;
            }

            return _words;
        }

        private string NumbersToWords(decimal numbertoconvert)
        {
            string _number = (numbertoconvert == 1000000000000000? "1000000000000000":numbertoconvert.ToString());
            string _words = "";

            int _digits = _number.Length.WholePartDivision(3);

            if ((_digits > 16) || (numbertoconvert > 1000000000000000)) _words="Number was too long.";
            else 
            {
                char[] _numbers = _number.ToCharArray();
                int _current = 0;

                for (int i = 0; i <= _numbers.Length - 1; i++)
                {
                    _current = (_numbers.Length - 1) - i;

                    switch (_current)
                    {
                        case 15:
                            _words = OnesToWords(int.Parse(_numbers[i].ToString())) + " Quadrillion"; break;
                        case 14:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            _words += HundredsToWords(int.Parse(_numbers[i].ToString() + _numbers[i + 1].ToString() + _numbers[i + 2].ToString()));
                            break;
                        case 13:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            _words += TensToWords(int.Parse(_numbers[i].ToString() + _numbers[i + 1].ToString()));
                            break;
                        case 12:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            if (_number.Length == 13) _words += OnesToWords(int.Parse(_numbers[i].ToString())) + " Trillion";
                            else
                            {
                                _words += (int.Parse(_numbers[i - 1].ToString()) != 1? OnesToWords(int.Parse(_numbers[i].ToString())):"");
                                if (_number.Length == 14)
                                {
                                    if (int.Parse(_numbers[i - 1].ToString()) != 0) _words += (_words.Trim().EndsWith(" ")? "" : " ") + "Trillion";
                                }
                                else
                                {
                                    if (int.Parse(_numbers[i - 2].ToString() + _numbers[i - 1].ToString() + _numbers[i].ToString()) != 0) _words += (_words.Trim().EndsWith(" ") ? "" : " ") + "Trillion";
                                }
                            }
                            break;
                        case 11:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            _words += HundredsToWords(int.Parse(_numbers[i].ToString() + _numbers[i + 1].ToString() + _numbers[i + 2].ToString()));
                            break;
                        case 10:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            _words += TensToWords(int.Parse(_numbers[i].ToString() + _numbers[i + 1].ToString()));
                            break;
                        case 9:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            if (_numbers.Length == 10) _words += OnesToWords(int.Parse(_numbers[i].ToString())) + " Billion";
                            else
                            {
                                _words += (int.Parse(_numbers[i - 1].ToString()) != 1 ? OnesToWords(int.Parse(_numbers[i].ToString())) : "");
                                if (_number.Length == 11)
                                {
                                    if (int.Parse(_numbers[i - 1].ToString()) != 0) _words += (_words.Trim().EndsWith(" ") ? "" : " ") + "Billion";
                                }
                                else
                                {
                                    if (int.Parse(_numbers[i - 2].ToString() + _numbers[i - 1].ToString() + _numbers[i].ToString()) != 0) _words += (_words.Trim().EndsWith(" ") ? "" : " ") + "Billion";
                                }
                            }
                            break;
                        case 8:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            _words += HundredsToWords(int.Parse(_numbers[i].ToString() + _numbers[i + 1].ToString() + _numbers[i + 2].ToString()));
                            break;
                        case 7:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            _words += TensToWords(int.Parse(_numbers[i].ToString() + _numbers[i + 1].ToString()));
                            break;
                        case 6:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            if (_numbers.Length == 7) _words += OnesToWords(int.Parse(_numbers[i].ToString())) + " Million";
                            else
                            {
                                _words += (int.Parse(_numbers[i - 1].ToString()) != 1 ? OnesToWords(int.Parse(_numbers[i].ToString())) : "");
                                if (_number.Length == 8)
                                {
                                    if (int.Parse(_numbers[i - 1].ToString()) != 0) _words += (_words.Trim().EndsWith(" ") ? "" : " ") + "Million";
                                }
                                else
                                {
                                    if (int.Parse(_numbers[i - 2].ToString() + _numbers[i - 1].ToString() + _numbers[i].ToString()) != 0) _words += (_words.Trim().EndsWith(" ") ? "" : " ") + "Million";
                                }
                            }
                            break;
                        case 5:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            _words += HundredsToWords(int.Parse(_numbers[i].ToString() + _numbers[i + 1].ToString() + _numbers[i + 2].ToString()));
                            break;
                        case 4:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            _words += TensToWords(int.Parse(_numbers[i].ToString() + _numbers[i + 1].ToString()));
                            break;
                        case 3:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            if (_numbers.Length == 4) _words += OnesToWords(int.Parse(_numbers[i].ToString())) + " Thousand";
                            else
                            {
                                _words += (int.Parse(_numbers[i - 1].ToString()) != 1 ? OnesToWords(int.Parse(_numbers[i].ToString())) : "");
                                if (_number.Length == 5)
                                {
                                    if (int.Parse(_numbers[i - 1].ToString()) != 0) _words += (_words.Trim().EndsWith(" ") ? "" : " ") + "Thousand";
                                }
                                else
                                {
                                    if (int.Parse(_numbers[i - 2].ToString() + _numbers[i - 1].ToString() + _numbers[i].ToString()) != 0) _words += (_words.Trim().EndsWith(" ") ? "" : " ") + "Thousand";
                                }
                            }
                            break;
                        case 2:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            _words += HundredsToWords(int.Parse(_numbers[i].ToString() + _numbers[i + 1].ToString() + _numbers[i + 2].ToString()));
                            break;
                        case 1:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            _words += TensToWords(int.Parse(_numbers[i].ToString() + _numbers[i + 1].ToString()));
                            break;
                        case 0:
                            _words += (_words.Trim() != "" && !_words.Trim().EndsWith(" ") ? " " : "");
                            if (_numbers.Length == 1) _words += OnesToWords(int.Parse(_numbers[i].ToString()));
                            else _words += (int.Parse(_numbers[i].ToString()) != 1 ? OnesToWords(int.Parse(_numbers[i].ToString())) : "");
                            break;
                        default: break;
                    }
                }
            }

            return _words.Trim();
        }

        /// <summary>
        /// Converts the initialized numeric value into its english-word representation.
        /// </summary>
        /// <returns>Returns the english-word representation of the initialized numeric value.</returns>
        /// <example>
        /// <code source="..\Development.Materia\Examples\Example.cs" region="AmountToWordsConverter 03" language="cs" />
        /// <code source="..\Development.Materia\Development.Materia.VBExamples\Development.Materia.VBExamples\Example.vb" region="AmountToWordsConverter 03" language="vbnet" />
        /// </example>
        public override string ToString()
        {
            string _words = "";

            _words = NumbersToWords(decimal.Parse(Amount.WholePartDivision(1).ToString()));
            double _decimals = Amount - Amount.WholePartDivision(1);
            if (_decimals > 0)
            {
                int _cents = (int) (_decimals * 100);
                _words += " and " + _cents.ToString() + " / 100";
            }

            if (Currency.Trim() != "") _words += " " + Currency.ToUpper();

            return _words;
        }

        #endregion

        #region "static methods"

        /// <summary>
        /// Converts the initialized numeric value into its english-word representation.
        /// </summary>
        /// <param name="amountvalue">Numeric / amount value to convert</param>
        /// <returns>Returns the english-words representation of the specified numeric value.</returns>
        /// <example>
        /// <code source="..\Development.Materia\Examples\Example.cs" region="AmountToWordsConverter 01" language="cs" />
        /// <code source="..\Development.Materia\Development.Materia.VBExamples\Development.Materia.VBExamples\Example.vb" region="AmountToWordsConverter 01" language="vbnet" />
        /// </example>
        public static string AmountToWords(double amountvalue)
        { return AmountToWords(amountvalue, ""); }

        /// <summary>
        /// Converts the initialized numeric value into its english-word representation suffixed with the specified currency.
        /// </summary>
        /// <param name="amountvalue">Numeric / amount value to convert</param>
        /// <param name="amountcurrency">Suffixing curreny</param>
        /// <returns>Returns the english-words representation of the specified numeric value suffixed with the specified currency.</returns>
        /// <example>
        /// <code source="..\Development.Materia\Examples\Example.cs" region="AmountToWordsConverter 02" language="cs" />
        /// <code source="..\Development.Materia\Development.Materia.VBExamples\Development.Materia.VBExamples\Example.vb" region="AmountToWordsConverter 02" language="vbnet" />
        /// </example>
        public static string AmountToWords(double amountvalue, string amountcurrency)
        {
            string _words = "";

            AmountToWordsConverter _converter = new AmountToWordsConverter(amountvalue, amountcurrency);
            _words = _converter.ToString(); _converter=null; GC.Collect();

            return _words;
        }

        #endregion

    }
}
