Imports Metaphor.Extensions
Imports Metaphor.Persistence

Friend Class ItemVerbModel
    Implements IVerbModel

    Private ReadOnly item As IItem
    Private ReadOnly verb As IVerb

    Private Sub New(item As IItem, verb As IVerb)
        Me.item = item
        Me.verb = verb
    End Sub

    Public ReadOnly Property IsEnabled As Boolean Implements IVerbModel.IsEnabled
        Get
            Return verb.CanPerform(item, item.World.Avatar)
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IVerbModel.Name
        Get
            Return verb.Name
        End Get
    End Property

    Public Sub Perform() Implements IVerbModel.Perform
        item.World.ClearMessages()
        verb.Perform(item, item.World.Avatar)
    End Sub

    Friend Shared Function Create(item As IItem, verb As IVerb) As IVerbModel
        Return New ItemVerbModel(item, verb)
    End Function
End Class
