Imports Metaphor.Extensions
Imports Metaphor.Persistence

Friend Class AvatarNavigationModel
    Implements IAvatarNavigationModel

    Private ReadOnly avatar As ICharacter

    Private Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public Sub SetHeading(heading As Double) Implements IAvatarNavigationModel.SetHeading
        avatar.Location.SetHeading(heading)
        avatar.Look()
        avatar.DialogMode = String.Empty
    End Sub

    Public Sub SetSpeed(speed As Double) Implements IAvatarNavigationModel.SetSpeed
        avatar.Location.SetSpeed(speed)
        avatar.Look()
        avatar.DialogMode = String.Empty
    End Sub

    Friend Shared Function Create(avatar As ICharacter) As IAvatarNavigationModel
        Return New AvatarNavigationModel(avatar)
    End Function
End Class
