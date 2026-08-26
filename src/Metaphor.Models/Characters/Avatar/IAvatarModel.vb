Public Interface IAvatarModel
    Sub ShowStatus()
    Sub Look()
    Sub SetHeading(heading As Double)
    Sub SetSpeed(speed As Double)
    ReadOnly Property Inventory As IInventoryModel
    ReadOnly Property AvailableVerbs As IEnumerable(Of IVerbModel)
    ReadOnly Property DialogMode As String
End Interface
