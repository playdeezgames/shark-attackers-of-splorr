Imports Metaphor.Persistence

Friend Module LocationInitializationExtensions
#Region "Boat"
    Friend Sub InitializeBoat(location As ILocation)
    End Sub
#End Region
#Region "Pier"
    Friend Function InitializePier(chosenName As String) As Persistence.LocationInitializer
        Return Sub(pier)
                   pier.CreateN00b(chosenName)
                   Dim boat = pier.World.CreateBoat()
                   boat.Moor(pier)
               End Sub
    End Function
#End Region
End Module
