Imports Metaphor.Extensions
Imports Metaphor.Persistence

Friend Class ItemModel
    Implements IItemModel

    Private ReadOnly item As IItem

    Private Sub New(item As IItem)
        Me.item = item
    End Sub

    Public ReadOnly Property Name As String Implements IItemModel.Name
        Get
            Return item.Name
        End Get
    End Property

    Public ReadOnly Property Verbs As IEnumerable(Of IVerbModel) Implements IItemModel.Verbs
        Get
            Return item.Verbs.Select(Function(x) ItemVerbModel.Create(item, x))
        End Get
    End Property

    Public Sub Take() Implements IItemModel.Take
        Dim world = item.World
        Dim character = world.Avatar
        world.ClearMessages()
        character.World.AddMessage($"{character.Name} takes {item.Name}.")
        item.Container = character.Inventory
    End Sub

    Public Sub Drop() Implements IItemModel.Drop
        Dim world = item.World
        Dim character = world.Avatar
        world.ClearMessages()
        character.World.AddMessage($"{character.Name} drops {item.Name}.")
        item.Container = character.Location.Inventory
    End Sub

    Public Sub Describe() Implements IItemModel.Describe
        Dim world = item.World
        world.ClearMessages()
        item.Describe()
    End Sub

    Friend Shared Function Create(item As IItem) As IItemModel
        Return New ItemModel(item)
    End Function
End Class
