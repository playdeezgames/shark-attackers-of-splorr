Public Delegate Sub LocationInitializer(location As ILocation)
Public Interface ILocation
    Inherits IMetaphorEntity
    Function CreateCharacter(entitySubtype As String, name As String, Optional initialize As CharacterInitializer = Nothing) As ICharacter
    Function CreateFeature(featureSubtype As String, name As String, Optional initializer As FeatureInitializer = Nothing) As IFeature
    ReadOnly Property Features As IEnumerable(Of IFeature)
    ReadOnly Property HasFeatures As Boolean
    Function GetOtherCharacters(character As ICharacter) As IEnumerable(Of ICharacter)
    Function HasOtherCharacters(character As ICharacter) As Boolean
    ReadOnly Property Characters As IEnumerable(Of ICharacter)
End Interface
