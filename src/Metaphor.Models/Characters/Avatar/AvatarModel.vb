Imports Metaphor.Extensions
Imports Metaphor.Persistence

Friend Class AvatarModel
    Implements IAvatarModel

    Private ReadOnly avatar As ICharacter

    Private Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property Inventory As IInventoryModel Implements IAvatarModel.Inventory
        Get
            Return InventoryModel.Create(avatar.Inventory)
        End Get
    End Property

    Public ReadOnly Property AvailableVerbs As IEnumerable(Of IVerbModel) Implements IAvatarModel.AvailableVerbs
        Get
            Return avatar.Verbs.Select(Function(x) CharacterVerbModel.Create(avatar, x))
        End Get
    End Property

    Public ReadOnly Property DialogMode As String Implements IAvatarModel.DialogMode
        Get
            Return avatar.DialogMode
        End Get
    End Property

    Public Sub ShowStatus() Implements IAvatarModel.ShowStatus
        avatar.World.ClearMessages()
        avatar.ShowStatus()
    End Sub

    Public Sub Look() Implements IAvatarModel.Look
        avatar.World.ClearMessages()
        avatar.Look()
    End Sub

    Friend Shared Function Create(avatar As ICharacter) As IAvatarModel
        Return New AvatarModel(avatar)
    End Function
End Class
