Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module WorldExtensions
#Region "Abandoned House"
    <Extension>
    Private Sub CreateAbandonedHouse(world As IWorld, context As IInitializationContext)
        world.CreateLocation(
            LocationSubtypes.ABANDONED_HOUSE,
            "The Abandoned House",
            LocationInitializationExtensions.InitializeAbandonedHouse(context.ChosenName))
    End Sub
#End Region
    <Extension>
    Public Sub Initialize(world As IWorld, context As IInitializationContext)
        world.Clear()
        world.CreateAbandonedHouse(context)
        world.AddMessage("Welcome to Shark Attackers of SPLORR!!!")
        world.Avatar.Look()
    End Sub
End Module
