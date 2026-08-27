Public Interface IAvatarModel
    Sub ShowStatus()
    Sub Look()
    ReadOnly Property Navigation As IAvatarNavigationModel
    ReadOnly Property Inventory As IInventoryModel
    ReadOnly Property AvailableVerbs As IEnumerable(Of IVerbModel)
    ReadOnly Property DialogMode As String
    ReadOnly Property Combat As IAvatarCombatModel
    ReadOnly Property IsDead As Boolean
End Interface
