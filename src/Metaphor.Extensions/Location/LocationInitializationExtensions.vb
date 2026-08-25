Imports Metaphor.Persistence

Friend Module LocationInitializationExtensions
#Region "Abandoned House"
    Friend Function InitializeAbandonedHouse(chosenName As String) As Persistence.LocationInitializer
        Return Sub(room)
                   room.CreateStainedMattress()
                   room.CreateN00b(chosenName)
                   room.CreateDoors(room.World.CreateOutside())
               End Sub
    End Function
#End Region
#Region "Outside"
    Friend Sub InitializeOutside(location As ILocation)
        location.CreateDoors(location.World.CreateDarkAlley())
        location.CreateBin()
        Dim payToilet = location.World.CreatePayToilet()
        Dim payDoor = location.CreateDoor($"Door to {payToilet.Name}", payToilet)
        payDoor.SetCounter(Counters.CASH, 1)
        payToilet.CreateDoor($"Door to {location.Name}", location)
    End Sub
#End Region
#Region "Dark Alley"
    Friend Sub InitializeDarkAlley(location As ILocation)
        location.CreateVerb(VerbSubtypes.SOLICIT, "Solicit")
    End Sub
#End Region
#Region "Pay Toilet"
    Friend Sub InitializePayToilet(location As ILocation)
        location.CreateSink()
    End Sub
#End Region
End Module
