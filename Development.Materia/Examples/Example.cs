using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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

        #region "Materia 01"

        private void form_load(object sender, EventArgs e)
        {
            if (sender != null) ((Form)sender).Text = "Loaded";
        }

        private void MateriaAttachHandler()
        {
            Form _form = new Form();
            Materia.AttachHandler(_form, "Load", new Action<object, EventArgs>(form_load));
        }

        #endregion

        private void ArchiverSample01()
        {
            #region "Archiver 01"

            Archiver _archiver = new Archiver(Application.StartupPath + "\\my folder to archive", 
                                 ArchivingToolEnum.SevenZip, ProcessWindowStyle.Hidden);
           
            System.IO.FileInfo _archivefile = _archiver.Archive();

            #endregion
        }

        private void ArchiverSample02()
        {
            #region "Archiver 02"

            System.IO.FileInfo _archivefile = Archiver.CompressAdd(Application.StartupPath + "\\my folder to archive");

            #endregion
        }

        private void ArchiverSample03()
        {
            #region "Archiver 03"

            System.IO.FileInfo _archivefile = Archiver.CompressAdd(Application.StartupPath + "\\my folder to archive",
                                                                   ArchivingToolEnum.SevenZip);

            #endregion
        }

        private void ArchiverSample04()
        {
            #region "Archiver 04"

            System.IO.FileInfo _archivefile = Archiver.CompressInsert (Application.StartupPath + "\\my folder to archive");

            #endregion
        }

        private void ArchiverSample05()
        {
            #region "Archiver 05"

            System.IO.FileInfo _archivefile = Archiver.CompressInsert(Application.StartupPath + "\\my folder to archive",
                                                                      ArchivingToolEnum.SevenZip);

            #endregion
        }
    }
}
