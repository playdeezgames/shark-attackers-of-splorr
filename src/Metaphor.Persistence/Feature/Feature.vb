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

    Public Property Destination As ILocation Implements IFeature.Destination
        Get
            Return World.GetLocation(GetYoke(Yokes.DESTINATION))
        End Get
        Set(value As ILocation)
            If value IsNot Nothing Then
                SetYoke(Yokes.DESTINATION, value.EntityId)
            Else
                ClearYoke(Yokes.DESTINATION)
            End If
        End Set
    End Property

    Public Property Twin As IFeature Implements IFeature.Twin
        Get
            Return World.GetFeature(GetYoke(Yokes.TWIN))
        End Get
        Set(value As IFeature)
            If value IsNot Nothing Then
                SetYoke(Yokes.TWIN, value.EntityId)
            Else
                ClearYoke(Yokes.TWIN)
            End If
        End Set
    End Property

    Protected Overrides ReadOnly Property Data As EntityData
        Get
            Return _data.Entities(EntityId)
        End Get
    End Property

    Public Overrides Sub Remove()
        If Not Exists Then
            Return
        End If
        Location.RemoveFromYokage(Yokages.FEATURES, EntityId)
        For Each verb In Verbs
            verb.Remove()
        Next
        Inventory.Remove()
        Dim twin = Me.Twin
        _data.Entities.Remove(EntityId)
        twin?.Remove()
    End Sub

    Friend Shared Function Create(world As IWorld, data As WorldData, featureId As Guid?) As IFeature
        If featureId.HasValue Then
            Return New Feature(world, data, featureId.Value)
        End If
        Return Nothing
    End Function
End Class
