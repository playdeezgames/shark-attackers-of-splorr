Public Interface IItemStack
    ReadOnly Property Container As IInventory
    ReadOnly Property ItemType As String
    ReadOnly Property Items As IEnumerable(Of IItem)
    ReadOnly Property Count As Integer
    ReadOnly Property Top As IItem
End Interface
