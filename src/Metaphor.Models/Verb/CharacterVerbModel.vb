Imports Metaphor.Extensions
Imports Metaphor.Persistence

Friend Class CharacterVerbModel
    Implements IVerbModel

    Private ReadOnly character As ICharacter
    Private ReadOnly verb As IVerb

    Private Sub New(character As ICharacter, verb As IVerb)
        Me.character = character
        Me.verb = verb
    End Sub

    Public ReadOnly Property IsEnabled As Boolean Implements IVerbModel.IsEnabled
        Get
            Return verb.CanPerform(character, character.World.Avatar)
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IVerbModel.Name
        Get
            Return verb.Name
        End Get
    End Property

    Public Sub Perform() Implements IVerbModel.Perform
        character.World.ClearMessages()
        verb.Perform(character, character.World.Avatar)
    End Sub

    Friend Shared Function Create(character As ICharacter, verb As IVerb) As IVerbModel
        Return New CharacterVerbModel(character, verb)
    End Function
End Class
