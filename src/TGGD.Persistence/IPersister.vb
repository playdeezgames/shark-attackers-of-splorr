Public Interface IPersister
    Function SaveAsync(filename As String, content As String) As Task
    Function LoadAsync(filename As String) As Task(Of String)
End Interface
