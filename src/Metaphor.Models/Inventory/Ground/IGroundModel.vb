Public Interface IGroundModel
    ReadOnly Property HasItems As Boolean
    ReadOnly Property Items As IEnumerable(Of IItemModel)
    ReadOnly Property Inventory As IInventoryModel
End Interface
