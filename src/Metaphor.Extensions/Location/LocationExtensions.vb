Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module LocationExtensions
#Region "N00b"
    <Extension>
    Friend Function CreateN00b(location As ILocation, name As String) As ICharacter
        Return location.CreateCharacter(CharacterSubtypes.N00B, name, AddressOf CharacterInitializationExtensions.InitializeN00b)
    End Function
#End Region
#Region "John"
    <Extension>
    Friend Function CreateJohn(location As ILocation) As ICharacter
        Return location.CreateCharacter(CharacterSubtypes.JOHN, "John", AddressOf CharacterInitializationExtensions.InitializeJohn)
    End Function
#End Region
#Region "Door"
    <Extension>
    Friend Function CreateDoor(location As ILocation, name As String, destination As ILocation) As IFeature
        Return location.CreateFeature(
            FeatureSubtypes.DOOR,
            name,
            Sub(feature)
                feature.SetDestination(destination)
                feature.CreateVerb(VerbSubtypes.ENTER, "Enter")
            End Sub)
    End Function
    <Extension>
    Friend Sub CreateDoors(fromLocation As ILocation, toLocation As ILocation)
        fromLocation.CreateDoor($"Door to {toLocation.Name}", toLocation)
        toLocation.CreateDoor($"Door to {fromLocation.Name}", fromLocation)
    End Sub
#End Region
#Region "Stained Mattress"
    <Extension>
    Friend Function CreateStainedMattress(location As ILocation) As IFeature
        Return location.CreateFeature(FeatureSubtypes.STAINED_MATTRESS, "Stained Mattress", AddressOf FeatureInitializationExtensions.InitializeStainedMattress)
    End Function
#End Region
#Region "Sink"
    <Extension>
    Friend Function CreateSink(location As ILocation) As IFeature
        Return location.CreateFeature(FeatureSubtypes.SINK, "Sink", AddressOf FeatureInitializationExtensions.InitializeSink)
    End Function
#End Region
#Region "Bin"
    <Extension>
    Friend Function CreateBin(location As ILocation) As IFeature
        Return location.CreateFeature(FeatureSubtypes.BIN, "Bin", AddressOf FeatureInitializationExtensions.InitializeBin)
    End Function
#End Region
End Module
