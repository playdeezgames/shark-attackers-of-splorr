Public Interface IDisplayContext
    ReadOnly Property Grid As IGrid
    Sub Render(
                   Optional text As String = Nothing,
                   Optional hints As IReadOnlyDictionary(Of String, String) = Nothing,
                   Optional newLine As Boolean = True)
End Interface
