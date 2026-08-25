Imports Metaphor.Persistence

Friend Module LocationInitializationExtensions
#Region "Boat"
    Friend Sub InitializeBoat(boat As ILocation)
        boat.SetXY(0.0, 0.0)
    End Sub
#End Region
#Region "Pier"
    Friend Function InitializePier(chosenName As String) As Persistence.LocationInitializer
        Return Sub(pier)
                   pier.SetXY(0.0, 0.0)
                   pier.CreateN00b(chosenName)
                   Dim boat = pier.World.CreateBoat()
                   boat.Moor(pier)
               End Sub
    End Function
#End Region
End Module
