Public Class DisplayElement
    Implements IDisplayElement

    Private Sub New(text As String, hints As IReadOnlyDictionary(Of String, String), newLine As Boolean)
        Me.Hints = hints
        Me.Text = text
        Me.NewLine = newLine
    End Sub

    Public ReadOnly Property Hints As IReadOnlyDictionary(Of String, String) Implements IDisplayElement.Hints

    Public ReadOnly Property Text As String Implements IDisplayElement.Text

    Public ReadOnly Property NewLine As Boolean Implements IDisplayElement.NewLine

    Public Shared Function Create(text As String, hints As IReadOnlyDictionary(Of String, String), newLine As Boolean) As IDisplayElement
        Return New DisplayElement(text, hints, newLine)
    End Function
End Class
