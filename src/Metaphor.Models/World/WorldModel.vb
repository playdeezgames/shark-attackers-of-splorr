Imports Metaphor.Extensions
Imports Metaphor.Persistence
Imports TGGD.Persistence
Imports TGGD.Processing

Public Class WorldModel
    Inherits BaseModel(Of IWorld)
    Implements IWorldModel

    Private Sub New(entity As IWorld, isQuittable As Boolean)
        MyBase.New(entity)
        Me.IsQuittable = isQuittable
    End Sub

    Public ReadOnly Property IsQuittable As Boolean Implements IWorldModel.IsQuittable

    Public ReadOnly Property Messages As IEnumerable(Of IMessage) Implements IWorldModel.Messages
        Get
            Return Entity.Messages
        End Get
    End Property

    Public ReadOnly Property Location As ILocationModel Implements IWorldModel.Location
        Get
            Return LocationModel.Create(Entity.Avatar.Location)
        End Get
    End Property

    Public ReadOnly Property Avatar As IAvatarModel Implements IWorldModel.Avatar
        Get
            Return AvatarModel.Create(Entity.Avatar)
        End Get
    End Property

    Public ReadOnly Property Ad As IAdModel Implements IWorldModel.Ad
        Get
            Return AdModel.Create(Entity)
        End Get
    End Property

    Public Sub Embark(chosenName As String, chosenPronouns As String) Implements IWorldModel.Embark
        Abandon()
        Entity.Initialize(InitializationContext.Create(chosenName))
    End Sub

    Public Sub Abandon() Implements IWorldModel.Abandon
        Entity.Clear()
    End Sub

    Public Shared Async Function Create(quittable As Boolean, persister As IPersister) As Task(Of IWorldModel)
        Dim entity As IWorld
        Try
            entity = Await Metaphor.Persistence.World.Load(SAVE_FILENAME, persister)
        Catch ex As Exception
            entity = Metaphor.Persistence.World.Create(New Provision.WorldData With {.EntityType = EntityTypes.WORLD_ENTITY}, persister)
        End Try
        Return New WorldModel(entity, quittable)
    End Function
End Class
