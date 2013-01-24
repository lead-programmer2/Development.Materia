using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Development.Materia
{
    class Example
    {

        private void AmountToWordsConverter01()
        {
            #region "AmountToWordsConverter 01"
            string _words = AmountToWordsConverter.AmountToWords(20.05);
            #endregion
        }

        private void AmountToWordsConverter02()
        {
            #region "AmountToWordsConverter 02"
            string _words = AmountToWordsConverter.AmountToWords(20.05, "USD");
            #endregion
        }

        private void AmountToWordsConverter03()
        {
            #region "AmountToWordsConverter 03"
            AmountToWordsConverter _converter = new AmountToWordsConverter(20.05, "USD");
            string _words = _converter.ToString();
            #endregion
        }
    }
}
