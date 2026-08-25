Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module FeatureExtensions
    Private Delegate Sub FeatureDescriber(feature As IFeature)
    Private ReadOnly describers As New Dictionary(Of String, FeatureDescriber) From
        {
        }
    Private Sub DescribeFeature(feature As IFeature)
        feature.AddMessage($"This is a {feature.Name}.")
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
End Module
