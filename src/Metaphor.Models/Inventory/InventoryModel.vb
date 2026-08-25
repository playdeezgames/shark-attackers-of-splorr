Imports Metaphor.Persistence

Friend Class InventoryModel
    Implements IInventoryModel

    Private ReadOnly inventory As IInventory

    Private Sub New(inventory As IInventory)
        Me.inventory = inventory
    End Sub

    Public ReadOnly Property HasItems As Boolean Implements IInventoryModel.HasItems
        Get
            Return inventory.HasItems
        End Get
    End Property

    Public ReadOnly Property Items As IEnumerable(Of IItemModel) Implements IInventoryModel.Items
        Get
            Return inventory.Items.Select(AddressOf ItemModel.Create)
        End Get
    End Property

    Public ReadOnly Property ItemStacks As IEnumerable(Of IItemStackModel) Implements IInventoryModel.ItemStacks
        Get
            Return inventory.ItemStacks.Select(AddressOf ItemStackModel.Create)
        End Get
    End Property

    Friend Shared Function Create(inventory As IInventory) As IInventoryModel
        Return New InventoryModel(inventory)
    End Function
End Class
