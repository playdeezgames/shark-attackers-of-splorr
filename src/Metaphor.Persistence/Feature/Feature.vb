Imports Metaphor.Provision
Imports TGGD.Provision

Friend Class Feature
    Inherits MetaphorEntity(Of EntityData)
    Implements IFeature

    Private Sub New(world As IWorld, data As WorldData, featureId As Guid)
        MyBase.New(world, data, featureId)
    End Sub

    Public ReadOnly Property Location As ILocation Implements IFeature.Location
        Get
            Return Persistence.Location.Create(World, _data, GetYoke(Yokes.LOCATION))
        End Get
    End Property

    Public Overrides ReadOnly Property Exists As Boolean
        Get
            Return _data.Entities.ContainsKey(EntityId)
        End Get
    End Property

    Protected Overrides ReadOnly Property Data As EntityData
        Get
            Return _data.Entities(EntityId)
        End Get
    End Property

    Public Overrides Sub Remove()
        Location.RemoveFromYokage(Yokages.FEATURES, EntityId)
        For Each verb In Verbs
            verb.Remove()
        Next
        Inventory.Remove()
        _data.Entities.Remove(EntityId)
    End Sub

    Friend Shared Function Create(world As IWorld, data As WorldData, featureId As Guid?) As IFeature
        If featureId.HasValue Then
            Return New Feature(world, data, featureId.Value)
        End If
        Return Nothing
    End Function
End Class
