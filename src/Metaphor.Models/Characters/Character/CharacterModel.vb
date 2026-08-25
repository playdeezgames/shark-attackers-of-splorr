Imports Metaphor.Persistence

Friend Class CharacterModel
    Implements ICharacterModel

    Private ReadOnly character As ICharacter

    Private Sub New(character As ICharacter)
        Me.character = character
    End Sub

    Public ReadOnly Property Name As String Implements ICharacterModel.Name
        Get
            Return character.Name
        End Get
    End Property

    Public ReadOnly Property AvailableVerbs As IEnumerable(Of IVerbModel) Implements ICharacterModel.AvailableVerbs
        Get
            Return character.Verbs.Select(Function(x) CharacterVerbModel.Create(character, x))
        End Get
    End Property

    Public ReadOnly Property Exists As Boolean Implements ICharacterModel.Exists
        Get
            Return character.Exists
        End Get
    End Property

    Public Sub Examine() Implements ICharacterModel.Examine
        Dim world = character.World
        world.ClearMessages()
    End Sub
    Friend Shared Function Create(character As ICharacter) As ICharacterModel
        Return New CharacterModel(character)
    End Function
End Class
