Public Delegate Sub ItemInitializer(item As IItem)
Public Interface IItem
    Inherits IMetaphorEntity
    Property Container As IInventory
End Interface
