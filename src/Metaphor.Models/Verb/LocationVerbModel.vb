Imports Metaphor.Extensions
Imports Metaphor.Persistence

Friend Class LocationVerbModel
    Implements IVerbModel

    Private ReadOnly location As ILocation
    Private ReadOnly verb As IVerb

    Private Sub New(location As ILocation, verb As IVerb)
        Me.location = location
        Me.verb = verb
    End Sub

    Public ReadOnly Property IsEnabled As Boolean Implements IVerbModel.IsEnabled
        Get
            Return verb.CanPerform(location, location.World.Avatar)
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IVerbModel.Name
        Get
            Return verb.Name
        End Get
    End Property

    Public Sub Perform() Implements IVerbModel.Perform
        location.World.ClearMessages()
        verb.Perform(location, location.World.Avatar)
    End Sub

    Friend Shared Function Create(location As ILocation, verb As IVerb) As IVerbModel
        Return New LocationVerbModel(location, verb)
    End Function
End Class
