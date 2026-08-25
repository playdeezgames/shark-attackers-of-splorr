Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module FeatureExtensions
    Private Delegate Sub FeatureDescriber(feature As IFeature)
    Private ReadOnly describers As New Dictionary(Of String, FeatureDescriber) From
        {
            {FeatureSubtypes.STAINED_MATTRESS, AddressOf DescribeStainedMattress},
            {FeatureSubtypes.DOOR, AddressOf DescribeDoor}
        }
    Private Sub DescribeFeature(feature As IFeature)
        feature.AddMessage($"This is a {feature.Name}.")
    End Sub

    Private Sub DescribeDoor(feature As IFeature)
        DescribeFeature(feature)
        Dim cash = feature.TryGetCounter(Counters.CASH)
        If cash.HasValue Then
            feature.AddMessage($"This costs {cash.Value} cash to go through.")
        End If
    End Sub

    Private Sub DescribeStainedMattress(feature As IFeature)
        DescribeFeature(feature)
        Dim filth = feature.TryGetCounter(Counters.FILTH)
        If filth.HasValue Then
            feature.AddMessage($"Sleeping here adds {filth.Value} filth.")
        End If
    End Sub

    <Extension>
    Public Sub Describe(feature As IFeature)
        Dim describer As FeatureDescriber = Nothing
        If describers.TryGetValue(feature.EntitySubtype, describer) Then
            describer.Invoke(feature)
        Else
            DescribeFeature(feature)
        End If
    End Sub
#Region "Destination"
    <Extension>
    Sub SetDestination(feature As IFeature, destination As ILocation)
        feature.SetYoke(Yokes.DESTINATION, destination.EntityId)
    End Sub
    <Extension>
    Function GetDestination(feature As IFeature) As ILocation
        Return feature.World.GetLocation(feature.GetYoke(Yokes.DESTINATION))
    End Function
#End Region
End Module
