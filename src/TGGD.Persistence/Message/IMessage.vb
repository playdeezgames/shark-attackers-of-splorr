Public Interface IMessage
    ReadOnly Property Text As String
    ReadOnly Property HintNames As IEnumerable(Of String)
    Function HasHint(hintName As String) As Boolean
    Function GetHint(hintName As String) As String
End Interface
