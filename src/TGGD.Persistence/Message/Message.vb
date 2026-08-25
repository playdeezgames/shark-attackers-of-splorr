Imports TGGD.Provision

Public Delegate Function MessageDataSource() As MessageData
Public Class Message
    Implements IMessage

    Private dataSource As MessageDataSource

    Private Sub New(dataSource As MessageDataSource)
        Me.dataSource = dataSource
    End Sub
    Public ReadOnly Property Text As String Implements IMessage.Text
        Get
            Return dataSource().Text
        End Get
    End Property

    Public ReadOnly Property HintNames As IEnumerable(Of String) Implements IMessage.HintNames
        Get
            Return If(dataSource().Hints?.Keys, Enumerable.Empty(Of String))
        End Get
    End Property

    Public Function HasHint(hintName As String) As Boolean Implements IMessage.HasHint
        Return If(dataSource().Hints?.ContainsKey(hintName), False)
    End Function

    Public Function GetHint(hintName As String) As String Implements IMessage.GetHint
        Dim result As String = Nothing
        If If(dataSource().Hints?.TryGetValue(hintName, result), False) Then
            Return result
        End If
        Return Nothing
    End Function

    Public Shared Function Create(dataSource As MessageDataSource) As IMessage
        Return New Message(dataSource)
    End Function
End Class
