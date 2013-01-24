Imports Development.Materia

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

End Class
