Imports TGGD.Persistence
Imports TGGD.Processing

Public Interface IWorldModel
    Inherits IModel
    ReadOnly Property IsQuittable As Boolean
    Sub Embark(chosenName As String, chosenPronouns As String)
    Sub Abandon()
    ReadOnly Property Ad As IAdModel
    ReadOnly Property Location As ILocationModel
    ReadOnly Property Avatar As IAvatarModel
    ReadOnly Property Messages As IEnumerable(Of IMessage)
End Interface
