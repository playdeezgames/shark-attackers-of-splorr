Imports Metaphor.Provision
Imports TGGD.Provision

Friend Class Character
    Inherits MetaphorEntity(Of EntityData)
    Implements ICharacter

    Private Sub New(world As IWorld, data As WorldData, characterId As Guid)
        MyBase.New(world, data, characterId)
    End Sub

    Public Property Location As ILocation Implements ICharacter.Location
        Get
            Return Persistence.Location.Create(World, _data, GetYoke(Yokes.LOCATION))
        End Get
        Set(value As ILocation)
            If value.EntityId <> Location.EntityId Then
                Location.RemoveFromYokage(Yokages.CHARACTERS, EntityId)
                SetYoke(Yokes.LOCATION, value.EntityId)
                Location.AddToYokage(Yokages.CHARACTERS, EntityId)
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property Exists As Boolean
        Get
            Return _data.Entities.ContainsKey(EntityId)
        End Get
    End Property

    Public Property DialogMode As String Implements ICharacter.DialogMode
        Get
            Return TryGetMetadata(Metadatas.DIALOG_MODE)
        End Get
        Set(value As String)
            SetMetadata(Metadatas.DIALOG_MODE, value)
        End Set
    End Property

    Protected Overrides ReadOnly Property Data As EntityData
        Get
            Return _data.Entities(EntityId)
        End Get
    End Property

    Public Overrides Sub Remove()
        Inventory.Remove()
        Location.RemoveFromYokage(Yokages.CHARACTERS, EntityId)
        _data.Entities.Remove(EntityId)
    End Sub

    Friend Shared Function Create(world As IWorld, data As WorldData, characterId As Guid?) As ICharacter
        Return If(characterId.HasValue, New Character(world, data, characterId.Value), Nothing)
    End Function
End Class
