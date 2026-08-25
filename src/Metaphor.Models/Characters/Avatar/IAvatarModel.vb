Public Interface IAvatarModel
    Sub ShowStatus()
    Sub Look()
    ReadOnly Property Inventory As IInventoryModel
    ReadOnly Property AvailableVerbs As IEnumerable(Of IVerbModel)
    ReadOnly Property DialogMode As String
    ReadOnly Property IsDead As Boolean
End Interface
