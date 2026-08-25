Public Delegate Sub CharacterInitializer(character As ICharacter)
Public Interface ICharacter
    Inherits IMetaphorEntity
    Property Location As ILocation
    Property DialogMode As String
End Interface
