Imports TGGD.Persistence

Public Interface IWorld
    Inherits IEntity
    Function Save(filename As String) As Task
    ReadOnly Property Messages As IEnumerable(Of IMessage)
    Sub ClearMessages()
    Sub AddMessage(text As String, Optional hints As IDictionary(Of String, String) = Nothing)
    Function CreateLocation(locationType As String, name As String, Optional initializer As LocationInitializer = Nothing) As ILocation
    Property Avatar As ICharacter
    Property AdFinish As DateTimeOffset?
    Function GetLocation(locationId As Guid?) As ILocation
    Function GetCharacter(characterId As Guid?) As ICharacter
    Function GetFeature(featureId As Guid?) As IFeature
End Interface
