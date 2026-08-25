Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module WorldExtensions
#Region "Pier"
    <Extension>
    Private Sub CreatePier(world As IWorld, context As IInitializationContext)
        world.CreateLocation(
            LocationSubtypes.PIER,
            "Pier",
            LocationInitializationExtensions.InitializePier(context.ChosenName))
    End Sub
    <Extension>
    Friend Function GetPier(world As IWorld) As ILocation
        Return world.GetLocation(world.GetYoke(Yokes.PIER))
    End Function
    <Extension>
    Friend Sub SetPier(world As IWorld, pier As ILocation)
        world.SetYoke(Yokes.PIER, pier.EntityId)
    End Sub
#End Region
#Region "Boat"
    <Extension>
    Friend Function CreateBoat(world As IWorld) As ILocation
        Return world.CreateLocation(
            LocationSubtypes.BOAT,
            "Blue Boat",
            AddressOf LocationInitializationExtensions.InitializeBoat)
    End Function
#End Region
    <Extension>
    Public Sub Initialize(world As IWorld, context As IInitializationContext)
        world.Clear()
        world.CreatePier(context)
        world.AddMessage("Welcome to Shark Attackers of SPLORR!!!")
        world.Avatar.Look()
    End Sub
End Module
