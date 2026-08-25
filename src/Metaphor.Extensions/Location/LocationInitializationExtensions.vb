Imports Metaphor.Persistence

Friend Module LocationInitializationExtensions
#Region "Boat"
    Friend Sub InitializeBoat(boat As ILocation)
        boat.SetXY(0.0, 0.0)
        boat.CreateVerb(VerbSubtypes.UNMOOR, "Unmoor")
        boat.CreateVerb(VerbSubtypes.MOOR, "Moor")
    End Sub
#End Region
#Region "Pier"
    Friend Function InitializePier(chosenName As String) As Persistence.LocationInitializer
        Return Sub(pier)
                   pier.SetXY(0.0, 0.0)
                   pier.CreateN00b(chosenName)
                   Dim boat = pier.World.CreateBoat()
                   boat.Moor(pier)
                   pier.World.SetPier(pier)
               End Sub
    End Function
#End Region
End Module
