Imports Metaphor.Extensions
Imports Metaphor.Persistence

Friend Class FeatureVerbModel
    Implements IVerbModel

    Private ReadOnly feature As IFeature
    Private ReadOnly verb As IVerb

    Private Sub New(feature As IFeature, verb As IVerb)
        Me.feature = feature
        Me.verb = verb
    End Sub

    Public ReadOnly Property IsEnabled As Boolean Implements IVerbModel.IsEnabled
        Get
            Return verb.CanPerform(feature, feature.World.Avatar)
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IVerbModel.Name
        Get
            Return verb.Name
        End Get
    End Property

    Public Sub Perform() Implements IVerbModel.Perform
        feature.World.ClearMessages()
        verb.Perform(feature, feature.World.Avatar)
    End Sub

    Friend Shared Function Create(feature As IFeature, verb As IVerb) As IVerbModel
        Return New FeatureVerbModel(feature, verb)
    End Function
End Class
