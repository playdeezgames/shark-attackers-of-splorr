Friend Class ItemStack
    Implements IItemStack

    Private Sub New(inventory As IInventory, itemType As String)
        Me.Container = inventory
        Me.ItemType = itemType
    End Sub

    Public ReadOnly Property Container As IInventory Implements IItemStack.Container

    Public ReadOnly Property ItemType As String Implements IItemStack.ItemType

    Public ReadOnly Property Items As IEnumerable(Of IItem) Implements IItemStack.Items
        Get
            Return Container.Items.Where(Function(x) x.EntitySubtype = ItemType)
        End Get
    End Property

    Public ReadOnly Property Count As Integer Implements IItemStack.Count
        Get
            Return Container.Items.Count(Function(x) x.EntitySubtype = ItemType)
        End Get
    End Property

    Public ReadOnly Property Top As IItem Implements IItemStack.Top
        Get
            Return Container.Items.FirstOrDefault(Function(x) x.EntitySubtype = ItemType)
        End Get
    End Property

    Friend Shared Function Create(inventory As IInventory, itemType As String) As IItemStack
        Return New ItemStack(inventory, itemType)
    End Function
End Class
