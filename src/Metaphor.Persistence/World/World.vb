Imports System.Text.Json
Imports Metaphor.Provision
Imports TGGD.Persistence
Imports TGGD.Provision

Public Class World
    Inherits Entity(Of WorldData)
    Implements IWorld

    Private Sub New(data As WorldData, persister As IPersister)
        Me.Data = data
        Me.persister = persister
    End Sub

    Public Overrides Sub Clear()
        MyBase.Clear()
        ClearMessages()
        Data.Entities.Clear()
        Data.AdFinishes = Nothing
    End Sub

    Protected Overrides ReadOnly Property Data As WorldData

    Public ReadOnly Property Messages As IEnumerable(Of IMessage) Implements IWorld.Messages
        Get
            Return Enumerable.
                Range(0, Data.Messages.Count).
                Select(Function(x) TGGD.Persistence.Message.Create(Function() Data.Messages(x)))
        End Get
    End Property

    Public Property Avatar As ICharacter Implements IWorld.Avatar
        Get
            Return Character.Create(Me, Data, GetYoke(Yokes.AVATAR))
        End Get
        Set(value As ICharacter)
            If value Is Nothing Then
                ClearYoke(Yokes.AVATAR)
            Else
                SetYoke(Yokes.AVATAR, value.EntityId)
            End If
        End Set
    End Property

    Public Property AdFinish As DateTimeOffset? Implements IWorld.AdFinish
        Get
            Return Data.AdFinishes
        End Get
        Set(value As DateTimeOffset?)
            Data.AdFinishes = value
        End Set
    End Property

    Private ReadOnly persister As IPersister

    Public Async Function Save(filename As String) As Task Implements IWorld.Save
        Await persister.SaveAsync(filename, JsonSerializer.Serialize(Data))
    End Function

    Public Shared Async Function Load(filename As String, persister As IPersister) As Task(Of IWorld)
        Return New World(JsonSerializer.Deserialize(Of WorldData)(Await persister.LoadAsync(filename)), persister)
    End Function

    Public Shared Function Create(data As WorldData, persister As IPersister) As IWorld
        Return New World(data, persister)
    End Function

    Public Sub ClearMessages() Implements IWorld.ClearMessages
        Data.Messages.Clear()
    End Sub

    Public Sub AddMessage(
                         text As String,
                         Optional hints As IDictionary(Of String, String) = Nothing) Implements IWorld.AddMessage
        Dim messageData As New MessageData With
            {
                .Text = text
            }
        If hints IsNot Nothing Then
            messageData.Hints = hints.ToDictionary(Function(x) x.Key, Function(x) x.Value)
        End If
        Data.Messages.Add(messageData)
    End Sub

    Public Function CreateLocation(entitySubtype As String, name As String, Optional initializer As LocationInitializer = Nothing) As ILocation Implements IWorld.CreateLocation
        Dim locationId = Guid.NewGuid
        Data.Entities(locationId) = New EntityData With
            {
                .EntityType = EntityTypes.LOCATION_ENTITY,
                .Metadatas = New Dictionary(Of String, String) From
                {
                    {Metadatas.ENTITY_SUBTYPE, entitySubtype},
                    {Metadatas.NAME, name}
                }
            }
        Dim result = Location.Create(Me, Data, locationId)
        initializer?.Invoke(result)
        Return result
    End Function

    Public Function GetLocation(locationId As Guid?) As ILocation Implements IWorld.GetLocation
        Return Location.Create(Me, Data, locationId)
    End Function

    Public Function GetCharacter(characterId As Guid?) As ICharacter Implements IWorld.GetCharacter
        Return Character.Create(Me, Data, characterId)
    End Function

    Public Function GetFeature(featureId As Guid?) As IFeature Implements IWorld.GetFeature
        Return Feature.Create(Me, Data, featureId)
    End Function
End Class
