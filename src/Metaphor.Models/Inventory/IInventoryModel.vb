Public Interface IInventoryModel
    ReadOnly Property HasItems As Boolean
    ReadOnly Property Items As IEnumerable(Of IItemModel)
    ReadOnly Property ItemStacks As IEnumerable(Of IItemStackModel)
End Interface
