Public Interface ICharacterModel
    ReadOnly Property Name As String
    Sub Examine()
    ReadOnly Property AvailableVerbs As IEnumerable(Of IVerbModel)
    ReadOnly Property Exists As Boolean
End Interface
