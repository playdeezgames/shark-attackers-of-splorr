Imports Metaphor.Persistence
Imports TGGD.Extensions

Friend Class ItemStackModel
    Implements IItemStackModel

    Private ReadOnly ItemStack As IItemStack

    Private Sub New(itemStack As IItemStack)
        Me.ItemStack = itemStack
    End Sub

    Public ReadOnly Property Top As IItemModel Implements IItemStackModel.Top
        Get
            Return ItemModel.Create(ItemStack.Top)
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IItemStackModel.Name
        Get
            Return $"{ItemStack.Top.Name}(x{ItemStack.Count})"
        End Get
    End Property

    Public ReadOnly Property Items As IEnumerable(Of IItemModel) Implements IItemStackModel.Items
        Get
            Return ItemStack.Items.Select(AddressOf ItemModel.Create)
        End Get
    End Property

    Public ReadOnly Property Count As Integer Implements IItemStackModel.Count
        Get
            Return ItemStack.Count
        End Get
    End Property

    Public Sub Drop(dropCount As Integer) Implements IItemStackModel.Drop
        dropCount = Math.Min(dropCount, ItemStack.Count)
        Dim world = ItemStack.Top.World
        Dim character = world.Avatar
        world.ClearMessages()
        character.World.AddMessage($"{character.Name} drops {dropCount} {ItemStack.Top.Name}.")
        Utility.Repeat(dropCount, Sub() ItemStack.Top.Container = character.Location.Inventory)
    End Sub

    Public Sub Take(takeCount As Integer) Implements IItemStackModel.Take
        takeCount = Math.Min(takeCount, ItemStack.Count)
        Dim world = ItemStack.Top.World
        Dim character = world.Avatar
        world.ClearMessages()
        character.World.AddMessage($"{character.Name} takes {takeCount} {ItemStack.Top.Name}.")
        Utility.Repeat(takeCount, Sub() ItemStack.Top.Container = character.Inventory)
    End Sub

    Friend Shared Function Create(itemStack As IItemStack) As IItemStackModel
        Return New ItemStackModel(itemStack)
    End Function
End Class
