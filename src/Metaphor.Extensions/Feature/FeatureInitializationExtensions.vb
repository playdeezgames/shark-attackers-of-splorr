Imports Metaphor.Persistence

Friend Module FeatureInitializationExtensions
    Friend Function InitializeMooring(toLocation As ILocation) As FeatureInitializer
        Return Sub(mooring)
                   mooring.Destination = toLocation
                   mooring.CreateVerb(VerbSubtypes.ENTER, "Enter")
               End Sub
    End Function
End Module
