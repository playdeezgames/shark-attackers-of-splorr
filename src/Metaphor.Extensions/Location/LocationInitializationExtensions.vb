Imports Metaphor.Persistence

Friend Module LocationInitializationExtensions
#Region "Boat"
    Friend Sub InitializeBoat(boat As ILocation)
        boat.SetXY(0.0, 0.0)
        boat.InitializeDimension(Dimensions.HEADING, 0.0, 0.0, 360.0)
        boat.InitializeDimension(Dimensions.SPEED, 1.0, 0.1, 1.0)
        boat.CreateVerb(VerbSubtypes.UNMOOR, "Unmoor")
        boat.CreateVerb(VerbSubtypes.MOOR, "Moor")
        boat.CreateVerb(VerbSubtypes.MOVE, "Move")
        boat.CreateVerb(VerbSubtypes.SET_HEADING, "Set Heading...")
        boat.CreateVerb(VerbSubtypes.SET_SPEED, "Set Speed...")
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
