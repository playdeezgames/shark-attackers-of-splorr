Imports TGGD.Provision

Public Class WorldData
    Inherits EntityData
    Public Property Entities As New Dictionary(Of Guid, EntityData)
    Public Property Messages As New List(Of MessageData)
    Public Property AdFinishes As DateTimeOffset?
End Class
