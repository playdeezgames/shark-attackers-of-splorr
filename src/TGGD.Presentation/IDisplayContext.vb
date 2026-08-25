Public Interface IDisplayContext
    Sub Render(
                   Optional text As String = Nothing,
                   Optional hints As IReadOnlyDictionary(Of String, String) = Nothing,
                   Optional newLine As Boolean = True)
End Interface
