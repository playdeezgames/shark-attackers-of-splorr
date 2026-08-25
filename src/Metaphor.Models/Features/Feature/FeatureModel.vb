Imports Metaphor.Extensions
Imports Metaphor.Persistence

Friend Class FeatureModel
    Implements IFeatureModel

    Private ReadOnly feature As IFeature

    Private Sub New(feature As IFeature)
        Me.feature = feature
    End Sub

    Public ReadOnly Property Name As String Implements IFeatureModel.Name
        Get
            Return feature.Name
        End Get
    End Property

    Public ReadOnly Property Verbs As IEnumerable(Of IVerbModel) Implements IFeatureModel.Verbs
        Get
            Return feature.Verbs.Select(Function(x) FeatureVerbModel.Create(feature, x))
        End Get
    End Property

    Public ReadOnly Property Exists As Boolean Implements IFeatureModel.Exists
        Get
            Return feature.Exists AndAlso feature.Location.EntityId = feature.World.Avatar.Location.EntityId
        End Get
    End Property

    Public ReadOnly Property Inventory As IInventoryModel Implements IFeatureModel.Inventory
        Get
            Return FeatureInventoryModel.Create(feature)
        End Get
    End Property

    Public ReadOnly Property Enabled As Boolean Implements IFeatureModel.Enabled
        Get
            Return Exists
        End Get
    End Property

    Public Sub Describe() Implements IFeatureModel.Describe
        feature.World.ClearMessages()
        feature.Describe()
    End Sub

    Friend Shared Function Create(feature As IFeature) As IFeatureModel
        Return New FeatureModel(feature)
    End Function
End Class
