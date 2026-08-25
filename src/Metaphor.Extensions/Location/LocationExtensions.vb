Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module LocationExtensions
#Region "N00b"
    <Extension>
    Friend Function CreateN00b(location As ILocation, name As String) As ICharacter
        Return location.CreateCharacter(CharacterSubtypes.N00B, name, AddressOf CharacterInitializationExtensions.InitializeN00b)
    End Function
#End Region
#Region "Moorings"
    <Extension>
    Private Function CreateMooring(fromLocation As ILocation, toLocation As ILocation) As IFeature
        Return fromLocation.CreateFeature(
            FeatureSubtypes.MOORING,
            $"Mooring from {fromLocation.Name} to {toLocation.Name}",
            FeatureInitializationExtensions.InitializeMooring(toLocation))
    End Function
    <Extension>
    Friend Sub Moor(fromLocation As ILocation, toLocation As ILocation)
        Dim toMooring = fromLocation.CreateMooring(toLocation)
        Dim fromMooring = toLocation.CreateMooring(fromLocation)
        toMooring.Twin = fromMooring
        fromMooring.Twin = toMooring
    End Sub
#End Region
End Module
