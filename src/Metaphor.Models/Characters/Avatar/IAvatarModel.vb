Public Interface IAvatarModel
    Sub ShowStatus()
    Sub Look()
    ReadOnly Property Navigation As IAvatarNavigationModel
    ReadOnly Property Inventory As IInventoryModel
    ReadOnly Property AvailableVerbs As IEnumerable(Of IVerbModel)
    ReadOnly Property DialogMode As String
End Interface
