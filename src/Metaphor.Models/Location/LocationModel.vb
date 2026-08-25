Imports Metaphor.Persistence

Friend Class LocationModel
    Implements ILocationModel

    Private ReadOnly location As ILocation

    Private Sub New(location As ILocation)
        Me.location = location
    End Sub

    Public ReadOnly Property AvailableVerbs As IEnumerable(Of IVerbModel) Implements ILocationModel.AvailableVerbs
        Get
            Return location.Verbs.Select(Function(x) LocationVerbModel.Create(location, x))
        End Get
    End Property

    Public ReadOnly Property OtherCharacters As IEnumerable(Of ICharacterModel) Implements ILocationModel.OtherCharacters
        Get
            Dim avatar = location.World.Avatar
            Return location.GetOtherCharacters(avatar).Select(AddressOf CharacterModel.Create)
        End Get
    End Property

    Friend Shared Function Create(location As ILocation) As ILocationModel
        Return New LocationModel(location)
    End Function

    Public ReadOnly Property Ground As IGroundModel Implements ILocationModel.Ground
        Get
            Return GroundModel.Create(location.World)
        End Get
    End Property

    Public ReadOnly Property Features As IFeaturesModel Implements ILocationModel.Features
        Get
            Return FeaturesModel.Create(location.World)
        End Get
    End Property

    Public ReadOnly Property Characters As ICharactersModel Implements ILocationModel.Characters
        Get
            Return CharactersModel.Create(location.World)
        End Get
    End Property
End Class
