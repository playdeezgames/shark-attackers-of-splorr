Imports Metaphor.Extensions
Imports Metaphor.Persistence

Friend Class AvatarCombatModel
    Implements IAvatarCombatModel

    Private ReadOnly avatar As ICharacter

    Private Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property Active As Boolean Implements IAvatarCombatModel.Active
        Get
            Return avatar.IsInCombat()
        End Get
    End Property

    Public Sub Fight() Implements IAvatarCombatModel.Fight
        avatar.World.ClearMessages()
        avatar.Fight()
    End Sub

    Friend Shared Function Create(avatar As ICharacter) As IAvatarCombatModel
        Return New AvatarCombatModel(avatar)
    End Function
End Class
