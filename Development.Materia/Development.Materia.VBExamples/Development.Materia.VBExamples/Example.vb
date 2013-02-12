Imports Development.Materia
Imports System.Windows.Forms

Public Class Example

    Private Sub AmountToWordsConverter01()
        ' #Region AmountToWordsConverter 01
        Dim _words As String = AmountToWordsConverter.AmountToWords(20.05)
        ' #End Region
    End Sub

    Private Sub AmountToWordsConverter02()
        ' #Region AmountToWordsConverter 02
        Dim _words As String = AmountToWordsConverter.AmountToWords(20.05, "USD")
        ' #End Region
    End Sub

    Private Sub AmountToWordsConverter03()
        ' #Region AmountToWordsConverter 03
        Dim _converter As New AmountToWordsConverter(20.05, "USD")
        Dim _words As String = _converter.ToString
        ' #End Region
    End Sub

    ' #Region Materia 01

    Private Sub form_load(ByVal sender As Object, ByVal e As EventArgs)
        If sender IsNot Nothing Then CType(sender, Form).Text = "Loaded"
    End Sub

    Private Sub MateriaAttachHandler()
        Dim _form As New Form
        Materia.AttachHandler(_form, "Load", New Action(Of Object, EventArgs)(AddressOf form_load))
    End Sub

    ' #End Region

    Private Sub ArchiverSample01()
        ' #Region Archiver 01

        Dim _archiver As New Archiver(Application.StartupPath & "\my folder to archive", _
                                      ArchivingToolEnum.SevenZip, ProcessWindowStyle.Hidden)

        Dim _archivefile As System.IO.FileInfo = _archiver.Archive

        ' #End Region
    End Sub

    Private Sub ArchiverSample02()
        ' #Region Archiver 02

        Dim _archivefile As System.IO.FileInfo = Archiver.CompressAdd(Application.StartupPath & "\my folder to archive")

        ' #End Region
    End Sub

    Private Sub ArchiverSample03()
        ' #Region Archiver 03

        Dim _archivefile As System.IO.FileInfo = Archiver.CompressAdd(Application.StartupPath & "\my folder to archive", _
                                                                      ArchivingToolEnum.SevenZip)

        ' #End Region
    End Sub

    Private Sub ArchiverSample04()
        ' #Region Archiver 04

        Dim _archivefile As System.IO.FileInfo = Archiver.CompressInsert(Application.StartupPath & "\my folder to archive")

        ' #End Region

    End Sub

    Private Sub ArchiverSample05()
        ' #Region Archiver 05

        Dim _archivefile As System.IO.FileInfo = Archiver.CompressInsert(Application.StartupPath & "\my folder to archive", _
                                                                         ArchivingToolEnum.SevenZip)

        ' #End Region
    End Sub

End Class
